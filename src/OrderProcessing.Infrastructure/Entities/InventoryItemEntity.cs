using System.ComponentModel.DataAnnotations;

namespace OrderProcessing.Infrastructure.Entities;

public class InventoryItemEntity : BaseEntity<Guid>
{
    public string Name { get; set; }
    public decimal Price { get; set; }
    public int StockQuantity { get; set; }
    public DateTimeOffset CreatedAt { get; private set; } = DateTimeOffset.UtcNow;
    public ICollection<InventoryReservationEntity> Reservations { get; set; }

    [Timestamp]
    public byte[] RowVersion { get; set; }
}
