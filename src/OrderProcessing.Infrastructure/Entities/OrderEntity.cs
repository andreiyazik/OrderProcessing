using OrderProcessing.Models.Order.Enums;

namespace OrderProcessing.Infrastructure.Entities;

public class OrderEntity : BaseEntity<Guid>
{
    public string Name { get; set; }
    public string Description { get; set; }
    public Status Status { get; set; }
    public DateTimeOffset CreatedAt { get; private set; } = DateTimeOffset.UtcNow;

    public ICollection<OrderItemEntity> Items { get; set; }
}
