namespace OrderProcessing.Domain.DTOs.Order;

public sealed class OrderItemCreateDto
{
    public Guid InventoryItemId { get; set; }
    public int Quantity { get; set; }
}
