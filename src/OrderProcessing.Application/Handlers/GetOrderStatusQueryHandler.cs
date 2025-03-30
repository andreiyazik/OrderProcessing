using MediatR;
using OrderProcessing.Application.Queries;
using OrderProcessing.Application.Repositories;
using OrderProcessing.Models.Order.Enums;

namespace OrderProcessing.Application.Handlers;

public class GetOrderStatusQueryHandler : IRequestHandler<GetOrderStatusQuery, Status>
{
    private readonly IOrderReadonlyRepository _orderRepository;

    public GetOrderStatusQueryHandler(IOrderReadonlyRepository orderRepository)
    {
        _orderRepository = orderRepository;
    }

    public async Task<Status> Handle(GetOrderStatusQuery request, CancellationToken cancellationToken)
    {
        var status = await _orderRepository.GetStatusAsync(request.OrderId, cancellationToken);

        return status;
    }
}
