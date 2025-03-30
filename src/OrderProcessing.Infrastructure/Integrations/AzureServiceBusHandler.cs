using Azure.Messaging.ServiceBus;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using OrderProcessing.Application.Integrations;
using OrderProcessing.Infrastructure.Configuration;
using Polly;
using System.Text.Json;

namespace OrderProcessing.Infrastructure.Integrations;

public class AzureServiceBusHandler : IServiceBusHandler, IAsyncDisposable
{
    private readonly ServiceBusClient _client;
    private readonly ServiceBusSender _sender;
    private readonly string _queueName;
    private readonly ILogger<AzureServiceBusHandler> _logger;

    public AzureServiceBusHandler(ServiceBusClient client,
        IOptions<ServiceBusSettings> options,
        ILogger<AzureServiceBusHandler> logger)
    {
        _client = client;
        _queueName = options.Value.QueueName;
        _sender = _client.CreateSender(_queueName);
        _logger = logger;
    }

    public async Task PublishMessageAsync<T>(T message)
    {
        try
        {
            var serializedMessage = JsonSerializer.Serialize(message);
            var serviceBusMessage = new ServiceBusMessage(serializedMessage);

            var retryPolicy = Policy
                .Handle<Exception>()
                .WaitAndRetryAsync(3, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)),
                    (exception, timeSpan, retryCount, context) =>
                    {
                        _logger.LogWarning("Retry {RetryCount} after {TimeSpan} due to: {Message}", retryCount, timeSpan, exception.Message);
                    });

            await retryPolicy.ExecuteAsync(async () =>
            {
                await _sender.SendMessageAsync(serviceBusMessage);
                _logger.LogInformation("Message published successfully to queue: {QueueName}", _queueName);
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to publish message to queue: {QueueName}", _queueName);
        }
    }

    public async Task ReceiveMessagesAsync(Func<string, Task> onEventReceived)
    {
        var processor = _client.CreateProcessor(_queueName, new ServiceBusProcessorOptions
        {
            AutoCompleteMessages = false,
            MaxConcurrentCalls = 5
        });

        processor.ProcessMessageAsync += async args =>
        {
            try
            {
                var body = args.Message.Body.ToString();
                await onEventReceived(body);
                await args.CompleteMessageAsync(args.Message);
                _logger.LogInformation("Message processed and completed successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to process message. Abandoning message.");
                await args.AbandonMessageAsync(args.Message);
            }
        };

        processor.ProcessErrorAsync += args =>
        {
            _logger.LogError(args.Exception, "Error occurred while processing messages.");
            return Task.CompletedTask;
        };

        await processor.StartProcessingAsync();
        _logger.LogInformation("Service Bus Processor started for queue: {QueueName}", _queueName);
    }

    public async ValueTask DisposeAsync()
    {
        await _client.DisposeAsync();
        _logger.LogInformation("Service Bus Client disposed.");
    }
}
