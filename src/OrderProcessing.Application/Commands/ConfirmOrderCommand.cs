using MediatR;
namespace OrderProcessing.Application.Commands;

public sealed record ConfirmOrderCommand(Guid OrderId) : IRequest;
