using OrderProcessing.Models.Order.Enums;

namespace OrderProcessing.Models.Order.Infos;

public sealed record ChangeOrderStatusInfo(Status NewStatus);
