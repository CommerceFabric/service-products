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
            connectionString = connectionString.Replace("$COMMERCEFABRIC_PRODUCTSERVICE_DB_NAME", Environment.GetEnvironmentVariable("COMMERCEFABRIC_PRODUCTSERVICE_DB_NAME") ?? "productDB");
            connectionString = connectionString.Replace("$COMMERCEFABRIC_PRODUCTSERVICE_DB_USER", Environment.GetEnvironmentVariable("COMMERCEFABRIC_PRODUCTSERVICE_DB_USER") ?? "root");
            connectionString = connectionString.Replace("$COMMERCEFABRIC_PRODUCTSERVICE_DB_PORT", Environment.GetEnvironmentVariable("COMMERCEFABRIC_PRODUCTSERVICE_DB_PORT") ?? "3306");

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
