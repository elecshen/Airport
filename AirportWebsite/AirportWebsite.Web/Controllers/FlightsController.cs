using Microsoft.AspNetCore.Mvc;
using AirportWebsite.Core.Interfaces;
using AirportWebsite.Web.ViewModels;

namespace AirportWebsite.Web.Controllers;

[Authorize]
public class FlightsController : Controller
{
    private readonly IFlightsService _flightsService;
    private readonly ILogger<FlightsController> _logger;

    public FlightsController(IFlightsService flightsService, ILogger<FlightsController> logger)
    {
        _flightsService = flightsService;
        _logger = logger;
    }

    [HttpGet]
    public async Task<IActionResult> Index(string? origin, string? destination, DateTime? departureDate)
    {
        var viewModel = new FlightSearchViewModel
        {
            Origin = origin,
            Destination = destination,
            DepartureDate = departureDate
        };

        if (!string.IsNullOrWhiteSpace(origin) || !string.IsNullOrWhiteSpace(destination) || departureDate.HasValue)
        {
            var flights = await _flightsService.SearchFlightsAsync(origin, destination, departureDate);
            viewModel.Flights = flights.ToList();
        }

        return View(viewModel);
    }

    [HttpGet]
    public async Task<IActionResult> Details(int id)
    {
        var flight = await _flightsService.GetFlightByIdAsync(id);
        
        if (flight == null)
        {
            return NotFound();
        }

        var viewModel = new FlightDetailsViewModel
        {
            Id = flight.Id,
            FlightNumber = flight.FlightNumber,
            Origin = flight.Origin,
            Destination = flight.Destination,
            DepartureTime = flight.DepartureTime,
            ArrivalTime = flight.ArrivalTime,
            Status = flight.Status,
            AvailableSeats = flight.AvailableSeats,
            BasePrice = flight.BasePrice,
            Gate = flight.Gate,
            Terminal = flight.Terminal
        };

        return View(viewModel);
    }
}
