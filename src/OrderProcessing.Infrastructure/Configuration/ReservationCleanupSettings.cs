namespace OrderProcessing.Infrastructure.Configuration;

public class ReservationCleanupSettings
{
    public double CleanupIntervalHours { get; set; }
    public double ReservationExpiryHours { get; set; }
}
