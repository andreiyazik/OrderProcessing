using OrderProcessing.Domain.DTOs.Order;
using OrderProcessing.Models.Order.Enums;

namespace OrderProcessing.Application.Repositories;

public interface IOrderReadonlyRepository
{
    Task<OrderDto> GetByIdAsync(Guid orderId, CancellationToken cancellationToken);
    Task<Status> GetStatusAsync(Guid orderId, CancellationToken cancellationToken);
}
