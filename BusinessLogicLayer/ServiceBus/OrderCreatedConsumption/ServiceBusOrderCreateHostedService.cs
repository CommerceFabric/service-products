using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Text;

namespace BusinessLogicLayer.ServiceBus.OrderCreatedConsumption
{

    public class ServiceBusOrderCreateHostedService : IHostedService
    {
        private readonly IServiceBusOrderCreateConsumer _consumer;

        public ServiceBusOrderCreateHostedService(IServiceBusOrderCreateConsumer consumer)
        {
            _consumer = consumer;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            await _consumer.ConsumeAsync(); // Start consuming messages from azure service bus when the hosted service starts
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            _consumer.Dispose(); // Dispose the consumer when the hosted service stops
        }
    }
}
