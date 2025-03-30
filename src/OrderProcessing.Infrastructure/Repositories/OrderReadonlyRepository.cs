using Microsoft.EntityFrameworkCore;
using OrderProcessing.Application.Repositories;
using OrderProcessing.Domain.DTOs.Order;
using OrderProcessing.Models.Order.Enums;

namespace OrderProcessing.Infrastructure.Repositories;

public class OrderReadonlyRepository : IOrderReadonlyRepository
{
    private readonly OrderDbContext _context;

    public OrderReadonlyRepository(OrderDbContext context)
    {
        _context = context;
    }

    public async Task<OrderDto> GetByIdAsync(Guid orderId, CancellationToken cancellationToken)
    {
        var order = await _context.Orders
            .AsNoTracking()
            .Select(order => new OrderDto
            {
                Id = order.Id,
                Name = order.Name,
                Description = order.Description,
                Status = order.Status
            })
            .FirstOrDefaultAsync(o => o.Id == orderId, cancellationToken);

        if (order is null)
            throw new Exception($"Order with Id {orderId} not found.");

        order.Items = await _context.OrderItems
            .AsNoTracking()
            .Include(orderItem => orderItem.InventoryItem)
            .Where(item => item.OrderId == orderId)
            .Select(item => new OrderItemDto
            {
                Id = item.Id,
                Name = item.InventoryItem.Name,
                Price = item.InventoryItem.Price,
                Quantity = item.Quantity
            })
            .ToListAsync(cancellationToken);

        return order;
    }

    public async Task<Status> GetStatusAsync(Guid orderId, CancellationToken cancellationToken)
    {
        var status = await _context.Orders
            .AsNoTracking()
            .Where(order => order.Id == orderId)
            .Select(order => order.Status)
            .FirstOrDefaultAsync(cancellationToken);

        return status;
    }
}
