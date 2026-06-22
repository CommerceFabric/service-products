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
            var test = configuration.GetConnectionString("DefaultConnection");

            services.AddDbContext<ApplicationDbContext>(options =>
            {
                options.UseMySql(
                    configuration.GetConnectionString("DefaultConnection")!,
                    ServerVersion.AutoDetect(configuration.GetConnectionString("DefaultConnection")!)
                );
            });

            services.AddScoped<IProductsRepository, ProductsRepository>();

            return services;
        }
    }
}
