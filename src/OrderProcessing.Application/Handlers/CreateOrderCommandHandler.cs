using AutoMapper;
using MediatR;
using OrderProcessing.Application.Commands;
using OrderProcessing.Application.Repositories;
using OrderProcessing.Domain.Aggregates.Order;
using OrderProcessing.Domain.DTOs.Order;

namespace OrderProcessing.Application.Handlers;

public class CreateOrderCommandHandler : IRequestHandler<CreateOrderCommand, Order>
{
    private readonly IOrderRepository _repository;
    private readonly IMapper _mapper;

    public CreateOrderCommandHandler(IOrderRepository repository, IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    public async Task<Order> Handle(CreateOrderCommand request, CancellationToken cancellationToken)
    {
        var dto = _mapper.Map<OrderCreateDto>(request.OrderInfo);

        var result = Order.CreateOrder(dto);
        if (!result.IsSucceeded)
            throw new Exception(string.Join(",", result.Errors.Select(e => e.Message)));

        await _repository.CreateAsync(result.Value, cancellationToken);

        return result.Value;
    }
}
