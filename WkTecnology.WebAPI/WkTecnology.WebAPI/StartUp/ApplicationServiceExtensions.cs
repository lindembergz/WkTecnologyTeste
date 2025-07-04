using Microsoft.Extensions.DependencyInjection;
using Portifolio.Aplicacao.Servicos;
using Portifolio.Dominio.Repositories; // se necessário
using Portifolio.Aplicacao.Validators;
using FluentValidation;

namespace WkTecnology.WebAPI.Extensions
{
    public static class ApplicationServiceExtensions
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {
            // Registrar serviços de aplicação
            services.AddScoped<IProductService, ProductService>();
            services.AddScoped<ICategoryService, CategoryService>();

            // Registrar validadores (assumindo que um dos validadores está na assembly do CreateProductDtoValidator)
            services.AddValidatorsFromAssemblyContaining<CreateProductDtoValidator>();

            return services;
        }
    }
}
