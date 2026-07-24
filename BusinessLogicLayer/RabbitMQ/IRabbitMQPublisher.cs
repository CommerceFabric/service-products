using System;
using System.Collections.Generic;
using System.Text;

namespace BusinessLogicLayer.RabbitMQ
{
    public interface IRabbitMQPublisher
    {
        Task Publish<ProductNameUpdateMessage>(string routingKey, ProductNameUpdateMessage message);
    }
}
