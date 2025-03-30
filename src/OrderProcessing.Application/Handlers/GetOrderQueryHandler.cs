using MediatR;
using Microsoft.Extensions.Caching.Memory;
using OrderProcessing.Application.Queries;
using OrderProcessing.Application.Repositories;
using OrderProcessing.Domain.DTOs.Order;

namespace OrderProcessing.Application.Handlers;

public class GetOrderQueryHandler : IRequestHandler<GetOrderQuery, OrderDto>
{
    private readonly IOrderReadonlyRepository _orderRepository;
    private readonly IMemoryCache _cache;

    public GetOrderQueryHandler(
        IOrderReadonlyRepository orderRepository,
        IMemoryCache cache)
    {
        _orderRepository = orderRepository;
        _cache = cache;
    }

    public async Task<OrderDto> Handle(GetOrderQuery request, CancellationToken cancellationToken)
    {
        var cacheKey = $"order_{request.OrderId}";

        if (_cache.TryGetValue(cacheKey, out OrderDto cachedOrder))
        {
            return cachedOrder;
        }

        var order = await _orderRepository.GetByIdAsync(request.OrderId, cancellationToken);

        if (order != null)
        {
            _cache.Set(cacheKey, order, TimeSpan.FromMinutes(10));
        }

        return order;
    }
}
