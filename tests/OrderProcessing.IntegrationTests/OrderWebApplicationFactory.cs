using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using OrderProcessing.Infrastructure;

namespace OrderProcessing.IntegrationTests;

public class OrderWebApplicationFactory<TStartup> : WebApplicationFactory<Program>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            var descriptor = services.SingleOrDefault(
                d => d.ServiceType == typeof(DbContextOptions<OrderDbContext>));
            if (descriptor != null)
                services.Remove(descriptor);

            services.AddDbContext<OrderDbContext>(options =>
            {
                options.UseSqlServer("Server=localhost,14333;Database=OrderTestDb;User=sa;Password=Your_password123;TrustServerCertificate=True;");
            });

            // Build the service provider
            var sp = services.BuildServiceProvider();

            // Create scope and apply migrations
            using var scope = sp.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<OrderDbContext>();
            context.Database.Migrate();
        });
    }
}
