namespace OrderProcessing.Models.Order.Views;

public sealed record OrderView(Guid Id, string Name, string Description, DateTimeOffset CreatedAt, StatusView Status, List<OrderItemView> Items);
