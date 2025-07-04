using Portifolio.Infraestrutura.Data;
using WkTecnology.WebAPI.Extensions;
using WkTecnology.WebAPI.StartUp;

var builder = WebApplication.CreateBuilder(args);

//Registra serviços da infraestrutura e aplicação (já existentes)
builder.Services.AddInfrastructureServices(builder.Configuration);
builder.Services.AddApplicationServices();

//Registra configurações customizadas:
builder.Services.AddCustomRateLimiter();
builder.Services.AddCustomCors();
builder.Services.AddCustomAntiforgery();


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
app.UseCustomCors();

//Middleware Antiforgery
app.UseCustomAntiforgery();
app.MapAntiforgeryTokenEndpoint();

//Rate Limiter e Controllers
app.UseCustomRateLimiter();
app.MapControllers();

await app.RunAsync();
