namespace OrderProcessing.Models.Order.Infos;

public sealed record OrderInfo(string Name, string Description, List<OrderItemInfo> Items);
