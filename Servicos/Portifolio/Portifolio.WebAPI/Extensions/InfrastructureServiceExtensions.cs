using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Portifolio.Dominio.Repositories;
using Portifolio.Infraestrutura.Data;
using Portifolio.Infraestrutura.Repositories;
using UserAuth.Infraestrutura.UoW;
using Portifolio.Infraestrutura.UoW;
using UserAuth.Dominio.Repositories;
using UserAuth.Infraestrutura.Repositories;
using UserAuth.Dominio.Servicos;
using UserAuth.Infraestrutura.Servicos;
using UserAuth.Aplicacao.Servicos;
using Portifolio.Dominio.UoW;
using UserAuth.Dominio.UoW;

namespace Portifolio.WebAPI.Extensions
{
    public static class InfrastructureServiceExtensions
    {
        public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration, IWebHostEnvironment environment)
        {
            if (!environment.IsEnvironment("Test"))
            {
                services.AddDbContext<PortifolioDbContext>(options =>
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

                services.AddDbContext<UserAuth.Infraestrutura.Data.UserDbContext>(options =>
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
            }

            services.AddScoped<IPortifolioUnitOfWork,PortifolioUnitOfWork>();
            services.AddScoped<IUserUnitOfWork, UserUnitOfWork>();
            services.AddScoped<IProductRepository, ProductRepository>();
            services.AddScoped<ICategoryRepository, CategoryRepository>();
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IEmailService, EmailService>();
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IAuthService, AuthService>();

            return services;
        }
    }
}