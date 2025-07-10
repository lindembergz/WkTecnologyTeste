using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Microsoft.AspNetCore.RateLimiting;
using Xunit;
using Portifolio.Infraestrutura.Data;
using WkTecnology.WebAPI;

namespace Portifolio.Tests
{
    public class CustomWebApplicationFactory : WebApplicationFactory<Program>
    {
        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.UseEnvironment("Test");

            builder.ConfigureServices(services =>
            {
                var descriptor = services.SingleOrDefault(
                    d => d.ServiceType == typeof(DbContextOptions<PortifolioDbContext>)
                );
                if (descriptor != null)
                {
                    services.Remove(descriptor);
                }

                services.AddDbContext<PortifolioDbContext>(options =>
                {
                options.UseInMemoryDatabase("TestDatabase");
                });
            });

            builder.ConfigureServices(services =>
            {
                services.AddRateLimiter(options =>
                {
                    options.RejectionStatusCode = 429;

                    options.AddFixedWindowLimiter("SearchPolicy", opt =>
                    {
                        opt.PermitLimit = 2; // Limite baixo para o teste
                        opt.Window = TimeSpan.FromMinutes(1);
                    });
                });
            });
        }
    }

    public class IntegrationTestBase : IClassFixture<CustomWebApplicationFactory>
    {
        protected readonly HttpClient _client;

        public IntegrationTestBase(CustomWebApplicationFactory factory)
        {
            _client = factory.CreateClient(new WebApplicationFactoryClientOptions { BaseAddress = new Uri("http://localhost:5284") });
        }
    }

    public class RateLimiterExtensionsTests : IntegrationTestBase
    {
        public RateLimiterExtensionsTests(CustomWebApplicationFactory factory) : base(factory)
        {
        }

        [Fact]
        public async Task RateLimiter_Should_Throttle_Excessive_Requests()
        {
            // Arrange
            int totalRequests = 6; // 6 requests to trigger throttling (no queuing in Test environment)

            // Act
            var requestTasks = new List<Task<HttpResponseMessage>>();
            for (int i = 0; i < totalRequests; i++)
            {
                requestTasks.Add(_client.GetAsync("/api/v1/Categories"));
            }
            HttpResponseMessage[] responses = await Task.WhenAll(requestTasks);


            // Assert
            int throttledCount = responses.Count(r => r.StatusCode == HttpStatusCode.TooManyRequests );
            var responseDetails = await Task.WhenAll(responses.Select(async r => $"{r.StatusCode}: {await r.Content.ReadAsStringAsync()}")).ConfigureAwait(false);
            Assert.True(throttledCount > 0, "Deveria haver pelo menos uma requisição com status 429 Too Many Requests. Respostas: " + string.Join(", ", responseDetails));
        }
    }
}