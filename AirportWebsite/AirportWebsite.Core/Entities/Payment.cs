using AirportWebsite.Core.Common;

namespace AirportWebsite.Core.Entities;

public class Payment : BaseEntity
{
    public int BookingId { get; set; }
    public string TransactionId { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public string CardLastFourDigits { get; set; } = string.Empty;
    public bool IsSuccess { get; set; }
    public int AttemptCount { get; set; } = 1;
    public DateTime? LastAttemptAt { get; set; }
    public string? ErrorMessage { get; set; }

    // Navigation property
    public Booking Booking { get; set; } = null!;
}
