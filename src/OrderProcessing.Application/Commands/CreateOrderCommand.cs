using MediatR;
using OrderProcessing.Domain.Aggregates.Order;
using OrderProcessing.Models.Order.Infos;

namespace OrderProcessing.Application.Commands;

public sealed record CreateOrderCommand(OrderInfo OrderInfo) : IRequest<Order>;
