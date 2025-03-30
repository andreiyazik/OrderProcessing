namespace OrderProcessing.Infrastructure.Entities;

public class InventoryReservationEntity : BaseEntity<Guid>
{
    public Guid InventoryItemId { get; set; }
    public Guid OrderId { get; set; }
    public int Quantity { get; set; }
    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;

    public InventoryItemEntity InventoryItem { get; set; }
    public OrderEntity Order { get; set; }
}
