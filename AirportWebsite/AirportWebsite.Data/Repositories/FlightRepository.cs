using AirportWebsite.Core.Entities;
using AirportWebsite.Core.Interfaces;
using Microsoft.EntityFrameworkCore;
using AirportWebsite.Data.Context;

namespace AirportWebsite.Data.Repositories;

public class FlightRepository : IFlightRepository
{
    private readonly ApplicationDbContext _context;

    public FlightRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Flight>> SearchFlightsAsync(
        string? origin, 
        string? destination, 
        DateTime? departureDate, 
        CancellationToken cancellationToken = default)
    {
        var query = _context.Flights
            .Where(f => !f.IsDeleted)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(origin))
        {
            query = query.Where(f => f.Origin.Contains(origin));
        }

        if (!string.IsNullOrWhiteSpace(destination))
        {
            query = query.Where(f => f.Destination.Contains(destination));
        }

        if (departureDate.HasValue)
        {
            var date = departureDate.Value.Date;
            query = query.Where(f => f.DepartureTime.Date == date);
        }

        return await query
            .OrderBy(f => f.DepartureTime)
            .ToListAsync(cancellationToken);
    }

    public async Task<Flight?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _context.Flights
            .FirstOrDefaultAsync(f => f.Id == id && !f.IsDeleted, cancellationToken);
    }

    public async Task<Flight?> GetByFlightNumberAsync(string flightNumber, CancellationToken cancellationToken = default)
    {
        return await _context.Flights
            .FirstOrDefaultAsync(f => f.FlightNumber == flightNumber && !f.IsDeleted, cancellationToken);
    }

    public async Task AddAsync(Flight flight, CancellationToken cancellationToken = default)
    {
        await _context.Flights.AddAsync(flight, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateAsync(Flight flight, CancellationToken cancellationToken = default)
    {
        flight.UpdatedAt = DateTime.UtcNow;
        _context.Flights.Update(flight);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task<bool> DecreaseAvailableSeatsAsync(int flightId, int seatsCount, CancellationToken cancellationToken = default)
    {
        var flight = await _context.Flights.FindAsync(new object[] { flightId }, cancellationToken);
        
        if (flight == null || flight.AvailableSeats < seatsCount)
        {
            return false;
        }

        flight.AvailableSeats -= seatsCount;
        flight.UpdatedAt = DateTime.UtcNow;
        
        try
        {
            await _context.SaveChangesAsync(cancellationToken);
            return true;
        }
        catch (DbUpdateConcurrencyException)
        {
            // Concurrency conflict - seats were modified by another transaction
            return false;
        }
    }
}
