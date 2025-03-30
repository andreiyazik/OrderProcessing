using MediatR;
using OrderProcessing.Models.Order.Enums;

namespace OrderProcessing.Application.Queries;

public sealed record GetOrderStatusQuery(Guid OrderId) : IRequest<Status>;
