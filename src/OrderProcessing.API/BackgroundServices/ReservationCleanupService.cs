using Microsoft.Extensions.Options;
using OrderProcessing.Infrastructure;
using OrderProcessing.Infrastructure.Configuration;

namespace OrderProcessing.API.BackgroundServices;

public class ReservationCleanupService : BackgroundService
{
    private readonly IServiceScopeFactory _serviceScopeFactory;
    private readonly ReservationCleanupSettings _settings;

    public ReservationCleanupService(
        IServiceScopeFactory serviceScopeFactory, 
        IOptions<ReservationCleanupSettings> settings)
    {
        _serviceScopeFactory = serviceScopeFactory;
        _settings = settings.Value;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var cleanupInterval = TimeSpan.FromHours(_settings.CleanupIntervalHours);

        while (!stoppingToken.IsCancellationRequested)
        {
            await CleanupOldReservationsAsync(stoppingToken);
            await Task.Delay(cleanupInterval, stoppingToken);
        }
    }

    private async Task CleanupOldReservationsAsync(CancellationToken cancellationToken)
    {
        using var scope = _serviceScopeFactory.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<OrderDbContext>();

        var cutoffTime = DateTimeOffset.UtcNow.AddHours(-_settings.ReservationExpiryHours);

        var oldReservations = context.InventoryReservations
            .Where(r => r.CreatedAt <= cutoffTime);

        context.InventoryReservations.RemoveRange(oldReservations);
        await context.SaveChangesAsync(cancellationToken);
    }
}
