using FluentAssertions;
using OrderProcessing.Models.Order.Enums;
using OrderProcessing.Models.Order.Views;
using System.Net.Http.Json;

namespace OrderProcessing.IntegrationTests.Tests;

public class OrderIntegrationTests : IClassFixture<OrderWebApplicationFactory<Program>>
{
    private readonly HttpClient _client;

    public OrderIntegrationTests(OrderWebApplicationFactory<Program> factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task CreateOrder_Then_GetOrder_ReturnsOrder()
    {
        // Arrange
        var inventoryItemId = Constants.FirstInventoryId;
        var order = new
        {
            Name = "Integration Order",
            Description = "From integration test",
            Items = new[]
            {
                new { InventoryItemId = inventoryItemId, Quantity = 2 }
            }
        };

        // Act
        var createResponse = await _client.PostAsJsonAsync("/orders", order);
        createResponse.EnsureSuccessStatusCode();
        var createdOrder = await createResponse.Content.ReadFromJsonAsync<CreatedOrderView>();

        // Assert Get
        var getResponse = await _client.GetAsync($"/orders/{createdOrder.Id}");
        getResponse.EnsureSuccessStatusCode();

        var fetchedOrder = await getResponse.Content.ReadFromJsonAsync<OrderView>();
        fetchedOrder.Should().NotBeNull();
        fetchedOrder.Id.Should().Be(createdOrder.Id);
        fetchedOrder.Items.Should().ContainSingle();
    }

    [Fact]
    public async Task CreateAndConfirmOrder_ShouldUpdateOrderStatusToConfirmed()
    {
        var inventoryItemId = Constants.FirstInventoryId;

        var order = new
        {
            Name = "Test Confirm Order",
            Description = "Integration test - confirm",
            Items = new[]
            {
                new { InventoryItemId = inventoryItemId, Quantity = 1 }
            }
        };

        // Act: Create Order
        var createResponse = await _client.PostAsJsonAsync("/orders", order);
        createResponse.EnsureSuccessStatusCode();

        var createdOrder = await createResponse.Content.ReadFromJsonAsync<CreatedOrderView>();
        createdOrder.Should().NotBeNull();

        // Act: Confirm the order
        var confirmResponse = await _client.PostAsync($"/orders/{createdOrder.Id}/confirm", null);
        confirmResponse.EnsureSuccessStatusCode();

        // Assert: Get the order status
        var getResponse = await _client.GetAsync($"/orders/{createdOrder.Id}/status");
        getResponse.EnsureSuccessStatusCode();

        var confirmedOrder = await getResponse.Content.ReadFromJsonAsync<StatusView>();
        confirmedOrder.Should().NotBeNull();
        confirmedOrder.Id.Should().Be((int)Status.Confirmed);
    }
}
