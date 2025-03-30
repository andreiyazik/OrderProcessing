namespace OrderProcessing.Infrastructure.Entities;

public abstract class BaseEntity<TId>
{
    public TId Id { get; set; }
}
