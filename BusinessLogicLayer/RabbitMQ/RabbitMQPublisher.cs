using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.Extensions.Configuration;
using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Text;

namespace BusinessLogicLayer.RabbitMQ
{
    public class RabbitMQPublisher : IRabbitMQPublisher, IDisposable
    {
        private readonly IConfiguration _configuration;
        private IChannel? _channel;
        private IConnection? _connection;
        private readonly SemaphoreSlim _lock = new(1, 1); // Semaphore to ensure thread safety when creating the channel (has 1 permit, so only one thread can enter at a time)

        public RabbitMQPublisher(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public void Dispose()
        {
            _channel?.Dispose();
            _connection?.Dispose();
        }

        public async Task Publish<ProductNameUpdateMessage>(string routingKey, ProductNameUpdateMessage message)
        {
            await EnsureConnectedAsync(); // Ensure that the channel is created and connected to RabbitMQ before publishing the message (has been lazy loaded)

            var body = Encoding.UTF8.GetBytes(System.Text.Json.JsonSerializer.Serialize(message)); // Serialize the message to JSON and convert it to a byte array
            
            string exchangeName = _configuration["RABBITMQ_PRODUCTS_EXCHANGE"]!; // the name of the exchange to publish to (eg products.exchange)

            // Declare the exchange
            await _channel!.ExchangeDeclareAsync(
                exchange: exchangeName, // the name of the exchange to declare (eg products.exchange)
                type: ExchangeType.Direct, // the type of the exchange (eg direct, fanout, topic, headers)
                durable: true // exchange should survive a broker restart
            );

            // Publish the message to the exchange with the specified routing key
            await _channel!.BasicPublishAsync(
                exchange: exchangeName, // the name of the exchange to publish to (eg products.exchange)
                routingKey: routingKey, // the routing key to use for the message (eg product.created, product.updated, etc.)
                body: body // the message body as a byte array
            );
        }

        /// <summary>
        /// Lazy initialization of the RabbitMQ channel. If the channel is already created, it returns immediately. Otherwise, it creates a new connection and channel to RabbitMQ.
        /// Required as it needs async methods to create the connection and channel, and we want to avoid creating them in the constructor.
        /// So instead, we create them on demand when the first message is published.
        /// Afterwhich, the channel is reused for subsequent messages.
        /// </summary>
        /// <returns></returns>
        private async Task EnsureConnectedAsync()
        {
            if (_channel != null) // if the channel is already created, return immediately (we have already lazy initialized the connection and channel for rabbitMQ)
                return;

            await _lock.WaitAsync(); // Use a semaphore to ensure that only one thread can create the connection and channel at a time (so that we don't create multiple connections and channels if multiple threads call Publish at the same time prior to the channel being initialized)


            try
            {
                if (_channel != null) // sanity check to see if the channel was created while waiting for the lock, if so, return immediately
                    return;

                // Create a new connection and channel to RabbitMQ
                var factory = new ConnectionFactory
                {
                    HostName = _configuration["RABBITMQ_HOST"]!,
                    UserName = _configuration["RABBITMQ_USER"]!,
                    Password = _configuration["RABBITMQ_PASSWORD"]!,
                    Port = int.Parse(_configuration["RABBITMQ_PORT"]!)
                };
                _connection = await factory.CreateConnectionAsync();
                _channel = await _connection.CreateChannelAsync();
            }
            finally
            {
                _lock.Release(); // Release the semaphore so that other threads can enter and use the channel
            }
        }
    }
}
