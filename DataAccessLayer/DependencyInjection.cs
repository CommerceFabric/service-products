using DataAccessLayer.Context;
using DataAccessLayer.Repositories;
using DataAccessLayer.RepositoryContracts;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace DataAccessLayer
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddDataAccessLayer(this IServiceCollection services, IConfiguration configuration)
        {
            // Get connection string and replace the placeholders with actual values from environment variables if they exist
            var connectionString = configuration.GetConnectionString("DefaultConnection")!;
            connectionString = connectionString.Replace("$COMMERCEFABRIC_PRODUCTSERVICE_DB_HOST", Environment.GetEnvironmentVariable("COMMERCEFABRIC_PRODUCTSERVICE_DB_HOST") ?? "localhost");
            connectionString = connectionString.Replace("$COMMERCEFABRIC_PRODUCTSERVICE_DB_PASSWORD", Environment.GetEnvironmentVariable("COMMERCEFABRIC_PRODUCTSERVICE_DB_PASSWORD") ?? "admin");

            // Add the ApplicationDbContext to the service collection with MySQL configuration
            services.AddDbContext<ApplicationDbContext>(options =>
            {
                options.UseMySql(
                    connectionString,
                    ServerVersion.AutoDetect(connectionString)
                );
            });

            services.AddScoped<IProductsRepository, ProductsRepository>();

            return services;
        }
    }
}
