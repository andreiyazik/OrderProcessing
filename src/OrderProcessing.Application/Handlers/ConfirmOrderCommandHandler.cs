using MediatR;
using Newtonsoft.Json.Linq;
using OrderProcessing.Application.Commands;
using OrderProcessing.Application.Integrations;
using OrderProcessing.Application.Repositories;
using OrderProcessing.Domain.Events;
using OrderProcessing.Models.Order.Enums;

namespace OrderProcessing.Application.Handlers;

public class ConfirmOrderCommandHandler : IRequestHandler<ConfirmOrderCommand>
{
    private readonly IOrderRepository _repository;
    private readonly IServiceBusHandler _serviceBusHandler;

    public ConfirmOrderCommandHandler(
        IOrderRepository repository,
        IServiceBusHandler serviceBusHandler)
    {
        _repository = repository;
        _serviceBusHandler = serviceBusHandler;
    }

    public async Task Handle(ConfirmOrderCommand request, CancellationToken cancellationToken)
    {
        var order = await _repository.GetByIdAsync(request.OrderId, cancellationToken);

        if (order is null)
            throw new Exception($"Order with Id {request.OrderId} not found.");

        order.ChangeStatus(Status.Confirmed);

        await _repository.UpdateAsync(order, cancellationToken);

        //** Uncomment the following code to publish the OrderConfirmed event to the Service Bus. Make sure to properly set up the Service Bus in appsettings **//.

        //var message = new ServiceBusMessage
        //{
        //    Name = "OrderConfirmed",
        //    Payload = JObject.FromObject(order)
        //};

        //await _serviceBusHandler.PublishMessageAsync(message);
    }
}
