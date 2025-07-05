using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace WkTecnology.WebAPI.StartUp
{
    public static class CorsExtensions
    {
        public static IServiceCollection AddCustomCors(this IServiceCollection services)
        {
            return services.AddCors(options =>
            {
                options.AddPolicy("AllowAngularApp", policy =>
                {
                    policy.WithOrigins("http://localhost:57852",
                                       "https://localhost:57852",
                                       "http://127.0.0.1:57852",
                                       "https://127.0.0.1:57852")
                          .AllowAnyHeader()
                          .AllowAnyMethod()
                          .AllowCredentials()
                          .WithExposedHeaders("X-Total-Count", "X-Page", "X-Page-Size", "X-Total-Pages");
                });
            });
        }

        public static IApplicationBuilder UseCustomCors(this IApplicationBuilder app)
        {
            return app.UseCors("AllowAngularApp");
        }

    }
}





