using Azure.Messaging.ServiceBus;
using System;
using System.Collections.Generic;
using System.Text;

namespace BusinessLogicLayer.ServiceBus.OrderCreatedConsumption
{
    public interface IServiceBusOrderCreateConsumer : IDisposable
    {
        public Task ConsumeAsync();
    }
}
