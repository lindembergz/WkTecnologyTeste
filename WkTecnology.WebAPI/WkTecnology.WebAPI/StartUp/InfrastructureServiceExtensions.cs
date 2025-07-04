using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Portifolio.Infraestrutura.Data;
using Portifolio.Dominio.Repositories;
using Portifolio.Infraestrutura.Repositories;
using ICiProvaCandidato.Dominio.UoW;
using System;

namespace WkTecnology.WebAPI.Extensions
{
    public static class InfrastructureServiceExtensions
    {
        public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
        {
            // Registrar DbContext
            services.AddDbContext<ApplicationDbContext>(options =>
            {
                options.UseMySql(
                    configuration.GetConnectionString("DefaultConnection"),
                    ServerVersion.AutoDetect(configuration.GetConnectionString("DefaultConnection")),
                    mysqlOptions =>
                    {
                        mysqlOptions.EnableRetryOnFailure(maxRetryCount: 3, maxRetryDelay: TimeSpan.FromSeconds(5), null);
                        mysqlOptions.CommandTimeout(30);
                    });
            });

          /* services.AddStackExchangeRedisCache(options =>
            {
                options.Configuration = builder.Configuration.GetConnectionString("Redis");
                options.InstanceName = "VehicleSales";
            });*/

            // Registrar UnitOfWork e repositórios
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddScoped<IProductRepository, ProductRepository>();
            services.AddScoped<ICategoryRepository, CategoryRepository>();

            return services;
        }
    }
}
