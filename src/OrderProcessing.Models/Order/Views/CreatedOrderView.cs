namespace OrderProcessing.Models.Order.Views;

public sealed record CreatedOrderView(Guid Id, string Name, string Description, DateTimeOffset CreatedAt, StatusView Status);
