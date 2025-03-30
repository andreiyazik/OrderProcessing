using MediatR;
using OrderProcessing.Domain.DTOs.Order;

namespace OrderProcessing.Application.Queries;

public sealed record GetOrderQuery(Guid OrderId) : IRequest<OrderDto>;
