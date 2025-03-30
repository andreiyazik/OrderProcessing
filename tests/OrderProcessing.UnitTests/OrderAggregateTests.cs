using OrderProcessing.Domain.Aggregates.Order;
using OrderProcessing.Domain.DTOs.Order;
using OrderProcessing.Models.Order.Enums;

namespace OrderProcessing.UnitTests;

public class OrderAggregateTests
{
    [Fact]
    public void CreateOrder_WithValidDto_ShouldSucceed()
    {
        // Arrange
        var dto = new OrderCreateDto
        {
            Name = "Test Order",
            Description = "Order Description",
            Items =
            [
                new() { InventoryItemId = Guid.NewGuid(), Quantity = 5 }
            ]
        };

        // Act
        var result = Order.CreateOrder(dto);

        // Assert
        Assert.True(result.IsSucceeded);
        Assert.NotNull(result.Value);
        Assert.Equal(Status.Pending, result.Value.Status);
        Assert.Single(result.Value.Items);
    }

    [Fact]
    public void CreateOrder_WithEmptyName_ShouldFail()
    {
        // Arrange
        var dto = new OrderCreateDto
        {
            Name = "",
            Description = "Order Description",
            Items =
            [
                new() { InventoryItemId = Guid.NewGuid(), Quantity = 1 }
            ]
        };

        // Act
        var result = Order.CreateOrder(dto);

        // Assert
        Assert.False(result.IsSucceeded);
        Assert.Contains(result.Errors, e => e.Message == "Order name cannot be empty.");
    }

    [Fact]
    public void CreateOrder_WithoutItems_ShouldFail()
    {
        // Arrange
        var dto = new OrderCreateDto
        {
            Name = "Test Order",
            Description = "Order Description",
            Items = []
        };

        // Act
        var result = Order.CreateOrder(dto);

        // Assert
        Assert.False(result.IsSucceeded);
        Assert.Contains(result.Errors, e => e.Message == "Order must contain at least one item.");
    }

    [Fact]
    public void ChangeStatus_FromPendingToConfirmed_ShouldSucceed()
    {
        // Arrange
        var dto = new OrderCreateDto
        {
            Name = "Valid Order",
            Description = "Description",
            Items =
            [
                new() { InventoryItemId = Guid.NewGuid(), Quantity = 2 }
            ]
        };

        var order = Order.CreateOrder(dto).Value;

        // Act
        var statusChangeResult = order.ChangeStatus(Status.Confirmed);

        // Assert
        Assert.True(statusChangeResult.IsSucceeded);
        Assert.Equal(Status.Confirmed, order.Status);
    }

    [Fact]
    public void ChangeStatus_InvalidTransition_ShouldFail()
    {
        // Arrange
        var dto = new OrderCreateDto
        {
            Name = "Valid Order",
            Description = "Description",
            Items =
            [
                new() { InventoryItemId = Guid.NewGuid(), Quantity = 2 }
            ]
        };

        var order = Order.CreateOrder(dto).Value;
        order.ChangeStatus(Status.Shipped);

        // Act
        var statusChangeResult = order.ChangeStatus(Status.Pending);

        // Assert
        Assert.False(statusChangeResult.IsSucceeded);
        Assert.Contains(statusChangeResult.Errors, e => e.Message.Contains("Cannot transition"));
    }
}
