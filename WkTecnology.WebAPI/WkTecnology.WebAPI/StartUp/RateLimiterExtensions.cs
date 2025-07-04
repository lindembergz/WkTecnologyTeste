using Microsoft.Extensions.DependencyInjection;
using System;
using Microsoft.AspNetCore.RateLimiting;
using System.Threading.RateLimiting;

namespace WkTecnology.WebAPI.StartUp
{
    public static class RateLimiterExtensions
    {
        public static IServiceCollection AddCustomRateLimiter(this IServiceCollection services)
        {
            services.AddRateLimiter(options =>
            {
                options.AddFixedWindowLimiter("DefaultPolicy", opt =>
                {
                    opt.PermitLimit = 100;
                    opt.Window = TimeSpan.FromMinutes(1);
                    opt.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
                    opt.QueueLimit = 50;
                });

                options.AddFixedWindowLimiter("SearchPolicy", opt =>
                {
                    opt.PermitLimit = 200;
                    opt.Window = TimeSpan.FromMinutes(1);
                });

                options.AddFixedWindowLimiter("CreatePolicy", opt =>
                {
                    opt.PermitLimit = 10;
                    opt.Window = TimeSpan.FromMinutes(1);
                });

                options.AddFixedWindowLimiter("UpdatePolicy", opt =>
                {
                    opt.PermitLimit = 20;
                    opt.Window = TimeSpan.FromMinutes(1);
                });

                options.AddFixedWindowLimiter("DeletePolicy", opt =>
                {
                    opt.PermitLimit = 5;
                    opt.Window = TimeSpan.FromMinutes(1);
                });
            });
            return services;
        }

        public static IApplicationBuilder UseCustomRateLimiter(this IApplicationBuilder app)
        {
            app.UseRateLimiter();
            return app;
        }
    }
}
