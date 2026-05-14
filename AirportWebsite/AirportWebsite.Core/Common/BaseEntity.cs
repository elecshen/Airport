namespace AirportWebsite.Core.Common;

public abstract class BaseEntity
{
    public int Id { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    public bool IsDeleted { get; set; } = false;
    
    // Concurrency token for optimistic concurrency control
    public byte[] RowVersion { get; set; } = Array.Empty<byte>();
}
