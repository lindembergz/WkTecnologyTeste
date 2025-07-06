
using Portifolio.Infraestrutura.Data;
using WkTecnology.WebAPI.Extensions;
using WkTecnology.WebAPI.Extension;

var builder = WebApplication.CreateBuilder(args);

//Registra servi�os da infraestrutura e aplica��o (j� existentes)
builder.Services.AddInfrastructureServices(builder.Configuration, builder.Environment);
builder.Services.AddApplicationServices();

//Registra configura��es customizadas:
builder.Services.AddCustomRateLimiter();
builder.Services.AddCustomCors();


if (!builder.Environment.IsEnvironment("Test"))
{
    builder.Services.AddCustomAntiforgery();
}

builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Title = "Vehicle Sales API",
        Version = "v1",
        Description = "High-performance vehicle sales management system",
        Contact = new Microsoft.OpenApi.Models.OpenApiContact
        {
            Name = "Development Team",
            Email = "dev@vehiclesales.com"
        }
    });
});

builder.Services.AddControllers();
builder.Services.AddLogging(logging =>
{
    logging.ClearProviders();
    logging.AddConsole();
    logging.AddDebug();
});
builder.Logging.AddFilter("Microsoft.AspNetCore.RateLimiting", Microsoft.Extensions.Logging.LogLevel.Debug);

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Vehicle Sales API V1");
        c.RoutePrefix = string.Empty;
    });
}

app.UseHttpsRedirection();
app.UseRouting();
app.UseCustomRateLimiter();
app.UseCustomCors();


//Middleware Antiforgery
if (!app.Environment.IsEnvironment("Test"))
{
    app.UseCustomAntiforgery();
    app.MapAntiforgeryTokenEndpoint();
}

//Controllers
app.MapControllers();

app.Run();

public partial class Program { }
