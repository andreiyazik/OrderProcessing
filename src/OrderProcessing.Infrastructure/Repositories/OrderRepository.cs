using AutoMapper;
using Azure.Core;
using Microsoft.EntityFrameworkCore;
using OrderProcessing.Application.Repositories;
using OrderProcessing.Domain.Aggregates.Order;
using OrderProcessing.Domain.DTOs.Order;
using OrderProcessing.Infrastructure.Entities;
using OrderProcessing.Infrastructure.Strategies;
using OrderProcessing.Models.Order.Enums;

namespace OrderProcessing.Infrastructure.Repositories;

public class OrderRepository : IOrderRepository
{
    private readonly OrderDbContext _context;
    private readonly IMapper _mapper;
    private readonly Dictionary<Status, IOrderUpdateStrategy> _strategies;

    public OrderRepository(OrderDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;

        _strategies = new Dictionary<Status, IOrderUpdateStrategy>
        {
            { Status.Confirmed, new ConfirmOrderUpdateStrategy() }
        };
    }

    public async Task CreateAsync(Order aggregate, CancellationToken cancellationToken)
    {
        using var transaction = await _context.Database.BeginTransactionAsync();

        try
        {
            foreach (var item in aggregate.Items)
            {
                var inventoryItem = await _context.InventoryItems
                    .FindAsync(item.InventoryItemId, cancellationToken);

                var reservedQuantity = await _context.InventoryReservations
                    .Where(r => r.InventoryItemId == item.InventoryItemId)
                    .SumAsync(r => r.Quantity);

                if (inventoryItem is null || inventoryItem.StockQuantity - reservedQuantity < item.Quantity)
                    throw new InvalidOperationException("Insufficient inventory.");

                var reservation = new InventoryReservationEntity
                {
                    Id = Guid.NewGuid(),
                    InventoryItemId = item.InventoryItemId,
                    OrderId = aggregate.Id,
                    Quantity = item.Quantity
                };

                _context.InventoryReservations.Add(reservation);
            }

            var entity = _mapper.Map<OrderEntity>(aggregate);
            _context.Orders.Add(entity);

            await _context.SaveChangesAsync();
            await transaction.CommitAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            await transaction.RollbackAsync();
            throw new InvalidOperationException("Concurrency conflict detected. Please retry the operation.");
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }
    }

    public async Task<Order> GetByIdAsync(Guid orderId, CancellationToken cancellationToken)
    {
        var order = await _context.Orders
            .Include(order => order.Items)
            .AsNoTracking()
            .FirstOrDefaultAsync(o => o.Id == orderId, cancellationToken);

        if (order is null)
            throw new Exception($"Order with Id {orderId} not found.");

        return _mapper.Map<Order>(order);
    }

    public async Task UpdateAsync(Order order, CancellationToken cancellationToken)
    {
        using var transaction = await _context.Database.BeginTransactionAsync();

        try
        {
            var currentOrderEntity = await _context.Orders.Include(o => o.Items).FirstOrDefaultAsync(o => o.Id == order.Id);

            if (currentOrderEntity == null)
                throw new InvalidOperationException($"Order with Id {order.Id} not found.");

            _mapper.Map(order, currentOrderEntity);

            if (_strategies.TryGetValue(order.Status, out var strategy))
            {
                await strategy.ExecuteAsync(currentOrderEntity, _context);
            }

            await _context.SaveChangesAsync();
            await transaction.CommitAsync();
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }
    }
}
