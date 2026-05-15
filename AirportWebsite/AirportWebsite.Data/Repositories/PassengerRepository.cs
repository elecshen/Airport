using AirportWebsite.Core.Entities;
using AirportWebsite.Core.Interfaces;
using Microsoft.EntityFrameworkCore;
using AirportWebsite.Data.Context;

namespace AirportWebsite.Data.Repositories;

public class PassengerRepository : IPassengerRepository
{
    private readonly ApplicationDbContext _context;

    public PassengerRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Passenger?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _context.Passengers
            .FirstOrDefaultAsync(p => p.Id == id && !p.IsDeleted, cancellationToken);
    }

    public async Task<Passenger?> FindByPassportAsync(string passportNumber, string passportCountry, CancellationToken cancellationToken = default)
    {
        return await _context.Passengers
            .FirstOrDefaultAsync(p => p.PassportNumber == passportNumber 
                                   && p.PassportCountry == passportCountry 
                                   && !p.IsDeleted, cancellationToken);
    }

    public async Task AddAsync(Passenger passenger, CancellationToken cancellationToken = default)
    {
        await _context.Passengers.AddAsync(passenger, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateAsync(Passenger passenger, CancellationToken cancellationToken = default)
    {
        passenger.UpdatedAt = DateTime.UtcNow;
        _context.Passengers.Update(passenger);
        await _context.SaveChangesAsync(cancellationToken);
    }
}
