using OrderProcessing.Domain.Aggregates.Order;
namespace OrderProcessing.Application.Repositories;

public interface IOrderRepository
{
    Task CreateAsync(Order aggregate, CancellationToken cancellationToken);
    Task<Order> GetByIdAsync(Guid orderId, CancellationToken cancellationToken);
    Task UpdateAsync(Order order, CancellationToken cancellationToken);
}
