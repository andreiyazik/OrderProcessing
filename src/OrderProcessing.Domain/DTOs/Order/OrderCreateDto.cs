namespace OrderProcessing.Domain.DTOs.Order;

public sealed class OrderCreateDto
{
    public string Name { get; set; }
    public string Description { get; set; }

    private DateTimeOffset? _createdAt;

    public DateTimeOffset CreatedAt {
        get => _createdAt ?? DateTimeOffset.UtcNow;
        set => _createdAt = value;
    }

    public IReadOnlyCollection<OrderItemCreateDto> Items { get; init; }
}
