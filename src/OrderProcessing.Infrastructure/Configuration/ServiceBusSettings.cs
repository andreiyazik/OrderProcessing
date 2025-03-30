namespace OrderProcessing.Infrastructure.Configuration;

public class ServiceBusSettings
{
    public string ConnectionString { get; set; }
    public string QueueName { get; set; }
}
