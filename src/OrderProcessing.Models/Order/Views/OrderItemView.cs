namespace OrderProcessing.Models.Order.Views;

public sealed record OrderItemView(Guid Id, string Name, int Quantity, decimal Price);
