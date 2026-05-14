using AirportWebsite.Core.Common;
using AirportWebsite.Core.Enums;

namespace AirportWebsite.Core.Entities;

public class Flight : BaseEntity
{
    public string FlightNumber { get; set; } = string.Empty;
    public string Origin { get; set; } = string.Empty;
    public string Destination { get; set; } = string.Empty;
    public DateTime DepartureTime { get; set; }
    public DateTime? ArrivalTime { get; set; }
    public FlightStatus Status { get; set; } = FlightStatus.Scheduled;
    public int TotalSeats { get; set; }
    public int AvailableSeats { get; set; }
    public decimal BasePrice { get; set; }
    public string? Gate { get; set; }
    public string? Terminal { get; set; }

    // Navigation property
    public ICollection<Booking> Bookings { get; set; } = new List<Booking>();
}
