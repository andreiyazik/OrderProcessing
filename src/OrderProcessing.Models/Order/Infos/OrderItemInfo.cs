namespace OrderProcessing.Models.Order.Infos;

public sealed record OrderItemInfo(Guid InventoryItemId, int Quantity);
