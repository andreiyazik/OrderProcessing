using OrderProcessing.Domain.DTOs.Order;

namespace OrderProcessing.Domain.Aggregates.Order;

public sealed class OrderItem : DomainEntity<Guid>
{
    public Guid InventoryItemId { get; set; }
    public int Quantity { get; set; }

    private OrderItem(Guid id, Guid inventoryItemId, int quantity) : base(id)
    {
        InventoryItemId = inventoryItemId;
        Quantity = quantity;
    }

    internal static OrderItem CreateOrderItem(OrderItemCreateDto i)
    {
        return new OrderItem(
            Guid.NewGuid(),
            i.InventoryItemId,
            i.Quantity);
    }
}
