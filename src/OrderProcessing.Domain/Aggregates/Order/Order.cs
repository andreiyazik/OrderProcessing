using OrderProcessing.Domain.DTOs.Order;
using OrderProcessing.Models.Common;
using OrderProcessing.Models.Order.Enums;

namespace OrderProcessing.Domain.Aggregates.Order;

public sealed class Order : DomainEntity<Guid>
{
    private readonly List<OrderItem> _items;

    public string Name { get; private set; }
    public string Description { get; private set; }
    public Status Status { get; private set; }
    public DateTimeOffset CreatedAt { get; private set; } = DateTimeOffset.UtcNow;
    public IReadOnlyCollection<OrderItem> Items => _items;


    private Order(Guid id, string name, string description) : base(id)
    {
        Name = name;
        Description = description;
        Status = Status.Pending;

        _items = [];
    }

    public static Result<Order> CreateOrder(OrderCreateDto dto)
    {
        var errors = new List<Error>();

        if (string.IsNullOrWhiteSpace(dto.Name))
            errors.Add(new Error("Order name cannot be empty."));

        if (!dto.Items.Any())
            errors.Add(new Error("Order must contain at least one item."));

        if (errors.Any())
            return new Result<Order>(errors);

        var order = new Order(
            Guid.NewGuid(),
            dto.Name,
            dto.Description);

        order._items.AddRange(dto.Items.Select(i => OrderItem.CreateOrderItem(i)));

        return new Result<Order>(order);
    }

    public Result<Order> ChangeStatus(Status newStatus)
    {
        var errors = ValidateStatusTransition(newStatus);
        if (errors.Any())
        {
            return new Result<Order>(errors);
        }

        Status = newStatus;

        return new Result<Order>(this);
    }

    private List<Error> ValidateStatusTransition(Status newStatus)
    {
        var errors = new List<Error>();

        var isValidTransition = Status switch
        {
            Status.Pending => newStatus == Status.Confirmed || newStatus == Status.Cancelled,
            Status.Confirmed => newStatus == Status.Shipped || newStatus == Status.Cancelled,
            Status.Shipped => false,
            Status.Cancelled => false,
            _ => false
        };

        if (!isValidTransition)
        {
            errors.Add(new Error($"Cannot transition from {Status} to {newStatus}."));
        }

        return errors;
    }
}
