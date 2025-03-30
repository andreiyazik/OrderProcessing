namespace OrderProcessing.Infrastructure.Entities;

public class OrderItemEntity : BaseEntity<Guid>
{
    public Guid OrderId { get; set; }
    public Guid InventoryItemId { get; set; }
    public int Quantity { get; set; }
    public DateTimeOffset CreatedAt { get; private set; } = DateTimeOffset.UtcNow;

    public OrderEntity Order { get; set; }
    public InventoryItemEntity InventoryItem { get; set; }
}
