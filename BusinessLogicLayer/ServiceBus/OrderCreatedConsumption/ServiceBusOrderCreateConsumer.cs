using Azure.Messaging.ServiceBus;
using BusinessLogicLayer.DTO;
using BusinessLogicLayer.RabbitMQ;
using BusinessLogicLayer.ServiceContracts;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;

namespace BusinessLogicLayer.ServiceBus.OrderCreatedConsumption
{
    public class ServiceBusOrderCreateConsumer : IServiceBusOrderCreateConsumer
    {
        private readonly ServiceBusProcessor _serviceBusProcessor;
        private readonly ServiceBusClient _serviceBusClient;
        private readonly IConfiguration _configuration;
        private readonly ILogger<ServiceBusOrderCreateConsumer> _logger;
        private readonly IServiceScopeFactory _scopeFactory;

        public ServiceBusOrderCreateConsumer(ServiceBusClient serviceBusClient, IConfiguration configuration, ILogger<ServiceBusOrderCreateConsumer> logger, IServiceScopeFactory scopeFactory)
        {
            _serviceBusClient = serviceBusClient;
            _configuration = configuration;
            _logger = logger;
            _scopeFactory = scopeFactory;

            // instantiate the ServiceBusProcessor with the topic and subscription from configuration
            var topic = _configuration["ProductsServiceBus:OrderCreatedTopic"];
            var subscription = _configuration["ProductsServiceBus:OrderCreatedProductsSubscription"];
            var options = new ServiceBusProcessorOptions
            {
                AutoCompleteMessages = false, // We will manually complete the messages after processing
            };
            _serviceBusProcessor = _serviceBusClient.CreateProcessor(topic, subscription, options);

            _serviceBusProcessor.ProcessMessageAsync += _serviceBusProcessor_ProcessMessageAsync;
            _serviceBusProcessor.ProcessErrorAsync += _serviceBusProcessor_ProcessErrorAsync;
        }

        private async Task _serviceBusProcessor_ProcessMessageAsync(ProcessMessageEventArgs arg)
        {
            var messageBodyJson = arg.Message.Body.ToString();
            var orderResponse = JsonSerializer.Deserialize<OrderResponse>(messageBodyJson);

            if (orderResponse != null)
            {
                // child scope
                using var scope = _scopeFactory.CreateScope();
                var productsService = scope.ServiceProvider.GetRequiredService<IProductsService>();

                var success = await HandleOrderCreation(orderResponse, productsService);

                // todo - in future should publish a message to notify orders service of success vs failure, so that it can update the order status accordingly
                if (!success)
                {
                    _logger.LogError($"Failed to handle order creation for OrderID: {orderResponse.OrderID}");
                    
                    // Throw to fail this message and trigger the retry mechanism of Service Bus.
                    //throw new Exception("Failed to handle order creation. This will cause the message to be retried or dead-lettered based on the Service Bus retry policy."); 
                    
                    // Force the message to be dead-lettered if the order creation handling fails, so that it can be retried or investigated later
                    await arg.DeadLetterMessageAsync(
                        arg.Message,
                        deadLetterReason: "InvalidOrder",
                        deadLetterErrorDescription:
                            $"Failed to handle order creation for OrderID: {orderResponse.OrderID}");

                }
            }

            // Log the successful processing of the message
            _logger.LogInformation($"Successfully processed order creation for OrderID: {orderResponse?.OrderID}");
            await arg.CompleteMessageAsync(arg.Message); // tell Service Bus that the message has been processed successfully
        }

        private async Task<bool> HandleOrderCreation(OrderResponse orderDTO, IProductsService productsService)
        {
            _logger.LogInformation($"Handling order creation for OrderID: {orderDTO.OrderID}; Placed On: {orderDTO.OrderDate.ToString()}");
            return await productsService.DecreaseProductStock(orderDTO.OrderItems);
        }

        private async Task _serviceBusProcessor_ProcessErrorAsync(ProcessErrorEventArgs arg)
        {
            _logger.LogError(arg.Exception, $"Service Bus Processor encountered an error: {arg.Exception.Message}");
        }

        public async Task ConsumeAsync()
        {
            await _serviceBusProcessor.StartProcessingAsync();
        }

        public async void Dispose()
        {
            await _serviceBusProcessor.StopProcessingAsync();
            await _serviceBusProcessor.DisposeAsync();
        }
    }
}
