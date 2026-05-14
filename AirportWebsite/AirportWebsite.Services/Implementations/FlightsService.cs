using AirportWebsite.Core.Entities;
using AirportWebsite.Core.Interfaces;
using Microsoft.Extensions.Logging;

namespace AirportWebsite.Services.Implementations;

public class FlightsService : IFlightsService
{
    private readonly IFlightRepository _flightRepository;
    private readonly ILogger<FlightsService> _logger;

    public FlightsService(
        IFlightRepository flightRepository,
        ILogger<FlightsService> logger)
    {
        _flightRepository = flightRepository;
        _logger = logger;
    }

    public async Task<IEnumerable<Flight>> SearchFlightsAsync(
        string? origin, 
        string? destination, 
        DateTime? departureDate, 
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Searching flights: Origin={Origin}, Destination={Destination}, Date={Date}", 
            origin, destination, departureDate);

        var flights = await _flightRepository.SearchFlightsAsync(origin, destination, departureDate, cancellationToken);
        
        _logger.LogInformation("Found {Count} flights", flights.Count());
        
        return flights;
    }

    public async Task<Flight?> GetFlightByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Getting flight by ID: {Id}", id);
        
        var flight = await _flightRepository.GetByIdAsync(id, cancellationToken);
        
        if (flight == null)
        {
            _logger.LogWarning("Flight with ID {Id} not found", id);
        }
        
        return flight;
    }
}
