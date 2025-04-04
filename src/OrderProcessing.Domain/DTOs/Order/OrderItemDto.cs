﻿namespace OrderProcessing.Domain.DTOs.Order;

public sealed class OrderItemDto
{
    public Guid Id { get; init; }
    public string Name { get; set; }
    public int Quantity { get; set; }
    public decimal Price { get; set; }
}
