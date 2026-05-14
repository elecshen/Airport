using AirportWebsite.Core.Common;
using AirportWebsite.Core.Enums;

namespace AirportWebsite.Core.Entities;

public class Booking : BaseEntity
{
    public string BookingReference { get; set; } = string.Empty;
    public int FlightId { get; set; }
    public int PassengerId { get; set; }
    public int SeatsCount { get; set; }
    public decimal TotalPrice { get; set; }
    public BookingStatus Status { get; set; } = BookingStatus.Pending;
    public DateTime? PaymentDeadline { get; set; }
    public DateTime? PaidAt { get; set; }
    public string? UserId { get; set; } // Link to Identity user

    // Navigation properties
    public Flight Flight { get; set; } = null!;
    public Passenger Passenger { get; set; } = null!;
    public Payment? Payment { get; set; }
}
