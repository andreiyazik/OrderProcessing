namespace OrderProcessing.Application.Integrations;

public interface IServiceBusHandler
{
    Task PublishMessageAsync<T>(T message);
    Task ReceiveMessagesAsync(Func<string, Task> onEventReceived);
}
