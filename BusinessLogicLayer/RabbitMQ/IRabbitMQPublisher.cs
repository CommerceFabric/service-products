using System;
using System.Collections.Generic;
using System.Text;

namespace BusinessLogicLayer.RabbitMQ
{
    public interface IRabbitMQPublisher
    {
        Task Publish<T>(string routingKey, T message);
    }
}
