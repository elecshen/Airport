using System.ComponentModel.DataAnnotations;

namespace AirportWebsite.Web.ViewModels;

public class FlightSearchViewModel
{
    [Display(Name = "Откуда")]
    public string? Origin { get; set; }

    [Display(Name = "Куда")]
    public string? Destination { get; set; }

    [Display(Name = "Дата вылета")]
    [DataType(DataType.Date)]
    public DateTime? DepartureDate { get; set; }

    public List<FlightListItemViewModel> Flights { get; set; } = new();
}

public class FlightListItemViewModel
{
    public int Id { get; set; }
    public string FlightNumber { get; set; } = string.Empty;
    public string Origin { get; set; } = string.Empty;
    public string Destination { get; set; } = string.Empty;
    public DateTime DepartureTime { get; set; }
    public DateTime? ArrivalTime { get; set; }
    public int AvailableSeats { get; set; }
    public decimal Price { get; set; }
    public string Status { get; set; } = string.Empty;

    public string DepartureTimeFormatted => DepartureTime.ToString("HH:mm");
    public string ArrivalTimeFormatted => ArrivalTime?.ToString("HH:mm") ?? "--:--";
    public string DateFormatted => DepartureTime.ToString("dd MMM yyyy");
}

public class FlightDetailsViewModel
{
    public int Id { get; set; }
    public string FlightNumber { get; set; } = string.Empty;
    public string Origin { get; set; } = string.Empty;
    public string Destination { get; set; } = string.Empty;
    public DateTime DepartureTime { get; set; }
    public DateTime? ArrivalTime { get; set; }
    public string Status { get; set; } = string.Empty;
    public int AvailableSeats { get; set; }
    public decimal BasePrice { get; set; }
    public string? Gate { get; set; }
    public string? Terminal { get; set; }
}
