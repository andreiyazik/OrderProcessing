using OrderProcessing.Models.Order.Enums;

namespace OrderProcessing.Domain.DTOs.Order;

public sealed class OrderDto
{
    public Guid Id { get; init; }
    public string Name { get; set; }
    public string Description { get; set; }
    public Status Status { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
    public IReadOnlyCollection<OrderItemDto> Items { get; set; }
}
