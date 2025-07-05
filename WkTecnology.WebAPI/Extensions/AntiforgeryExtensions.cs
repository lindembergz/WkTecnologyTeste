using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Http;

namespace WkTecnology.WebAPI.StartUp
{
    public static class AntiforgeryExtensions
    {
        public static IServiceCollection AddCustomAntiforgery(this IServiceCollection services, IWebHostEnvironment environment)
        {
            services.AddAntiforgery(options =>
            {
                options.HeaderName = "X-XSRF-TOKEN";
                options.Cookie.Name = "XSRF-TOKEN"; // Nome padrão para o Angular
                options.Cookie.SameSite = SameSiteMode.None;
                options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
                options.Cookie.Domain = "localhost"; // Ajuste conforme o domínio da API

                if (environment.IsEnvironment("Test"))
                {
                    options.Cookie.SecurePolicy = CookieSecurePolicy.None;
                }
            });
            return services;
        }

        public static IApplicationBuilder UseCustomAntiforgery(this IApplicationBuilder app)
        {
            app.Use(async (context, next) =>
            {
                if (HttpMethods.IsPost(context.Request.Method) ||
                    HttpMethods.IsPut(context.Request.Method) ||
                    HttpMethods.IsDelete(context.Request.Method))
                {
                    var antiforgery = context.RequestServices.GetRequiredService<IAntiforgery>();
                    await antiforgery.ValidateRequestAsync(context);
                }
                await next();
            });
            return app;
        }

        public static IEndpointRouteBuilder MapAntiforgeryTokenEndpoint(this IEndpointRouteBuilder endpoints)
        {
            endpoints.MapGet("/api/v1/antiforgery/token", async (HttpContext context, IAntiforgery antiforgery) =>
            {
                var tokens = antiforgery.GetAndStoreTokens(context);
                await context.Response.WriteAsJsonAsync(new { token = tokens.RequestToken });
            })
            .RequireCors("AllowAngularApp")
            .AllowAnonymous();

            return endpoints;
        }
    }
}
