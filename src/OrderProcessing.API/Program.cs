using Azure.Messaging.ServiceBus;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using OrderProcessing.API.BackgroundServices;
using OrderProcessing.API.Middleware;
using OrderProcessing.Application.Handlers;
using OrderProcessing.Application.Integrations;
using OrderProcessing.Application.Mapping;
using OrderProcessing.Application.Repositories;
using OrderProcessing.Infrastructure;
using OrderProcessing.Infrastructure.Configuration;
using OrderProcessing.Infrastructure.Integrations;
using OrderProcessing.Infrastructure.Mapping;
using OrderProcessing.Infrastructure.Repositories;
using Scalar.AspNetCore;
using Serilog;

Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .CreateLogger();

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog();

builder.Services.AddControllers();
builder.Services.AddOpenApi();

AddAutoMapper(builder);
AddServices(builder);
AddCaching(builder);
AddServiceBus(builder);
AddCors(builder);
AddHostedServices(builder);

var app = builder.Build();

app.UseMiddleware<ExceptionHandlingMiddleware>();

if (app.Environment.IsDevelopment())
{
    AddScalar(app);
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();

static void AddServiceBus(WebApplicationBuilder builder)
{
    builder.Services.Configure<ServiceBusSettings>(builder.Configuration.GetSection("ServiceBusSettings"));

    builder.Services.AddSingleton(provider =>
    {
        var settings = provider.GetRequiredService<IOptions<ServiceBusSettings>>().Value;
        return new ServiceBusClient(settings.ConnectionString);
    });

    builder.Services.AddSingleton<IServiceBusHandler, AzureServiceBusHandler>();
}

static void AddServices(WebApplicationBuilder builder)
{
    builder.Services.AddScoped<IOrderReadonlyRepository, OrderReadonlyRepository>();
    builder.Services.AddScoped<IOrderRepository, OrderRepository>();
    builder.Services.AddDbContext<OrderDbContext>(options =>
        options.UseSqlServer(builder.Configuration.GetConnectionString("OrdersDb")));

    builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(CreateOrderCommandHandler).Assembly));
}

static void AddAutoMapper(WebApplicationBuilder builder)
{
    builder.Services.AddAutoMapper(config =>
    {
        config.AddProfile<OrdersApplicationProfile>();
        config.AddProfile<OrdersProfile>();
    });
}

static void AddCors(WebApplicationBuilder builder)
{
    builder.Services.AddCors(options =>
    {
        options.AddPolicy("AllowSpecificOrigins", policy =>
        {
            policy.AllowAnyOrigin()
                  .AllowAnyHeader()
                  .AllowAnyMethod();
        });
    });
}

void AddHostedServices(WebApplicationBuilder builder)
{
    builder.Services.Configure<ReservationCleanupSettings>(
        builder.Configuration.GetSection("ReservationCleanupSettings"));

    builder.Services.AddHostedService<ReservationCleanupService>();
}

void AddCaching(WebApplicationBuilder builder)
{
    builder.Services.AddMemoryCache();
}

static void AddScalar(WebApplication app)
{
    app.MapScalarApiReference();
    app.MapOpenApi();
}

public partial class Program { }