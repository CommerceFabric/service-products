using Azure.Messaging.ServiceBus;
using BusinessLogicLayer.Mappers;
using BusinessLogicLayer.RabbitMQ;
using BusinessLogicLayer.ServiceBus;
using BusinessLogicLayer.ServiceContracts;
using BusinessLogicLayer.Services;
using FluentValidation;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace BusinessLogicLayer
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddBusinessLogicLayer(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddAutoMapper(
                cfg => { },
                typeof(ProductMappingProfile)
            );

            // Add Fluentvalidations to use as contract validators for the DTOs
            services.AddValidatorsFromAssembly(typeof(DependencyInjection).Assembly); // don't need to do this per validator, as it will automatically scan the assembly for all validators and register them in the DI container

            // Add services to the DI container
            services.AddScoped<IProductsService, ProductsService>();

            // Add RabbitMQ as singleton because we want to reuse the same connection and channel for publishing messages
            services.AddSingleton<IRabbitMQPublisher, RabbitMQPublisher>();

            // Add ServiceBus 
            var serviceBusConnectionString = configuration["ProductsServiceBus:ConnectionString"];
            serviceBusConnectionString = serviceBusConnectionString!.Replace("$SERVICEBUS_CONNECTION_STRING", 
                Environment.GetEnvironmentVariable("SERVICEBUS_CONNECTION_STRING") ?? string.Empty);
            services.AddSingleton( _ =>
            {
                return new ServiceBusClient(serviceBusConnectionString);
            });

            services.AddSingleton<IServiceBusPublisher, ServiceBusPublisher>();

            return services;
        }
    }
}
