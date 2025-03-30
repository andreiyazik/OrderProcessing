using OrderProcessing.Infrastructure.Entities;

namespace OrderProcessing.Infrastructure.Strategies;

public class ConfirmOrderUpdateStrategy : IOrderUpdateStrategy
{
    public async Task ExecuteAsync(OrderEntity order, OrderDbContext context)
    {
        foreach (var item in order.Items)
        {
            var inventoryItem = await context.InventoryItems.FindAsync(item.InventoryItemId);

            if (inventoryItem == null || inventoryItem.StockQuantity < item.Quantity)
                throw new InvalidOperationException("Insufficient inventory.");

            inventoryItem.StockQuantity -= item.Quantity;

            var reservations = context.InventoryReservations
                .Where(r => r.OrderId == order.Id && r.InventoryItemId == item.InventoryItemId);

            context.InventoryReservations.RemoveRange(reservations);
        }

        await context.SaveChangesAsync();
    }
}
