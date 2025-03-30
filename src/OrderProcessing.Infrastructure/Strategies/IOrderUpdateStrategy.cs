using OrderProcessing.Infrastructure.Entities;

namespace OrderProcessing.Infrastructure.Strategies;

public interface IOrderUpdateStrategy
{
    Task ExecuteAsync(OrderEntity order, OrderDbContext context);
}
