using AirportWebsite.Core.Entities;
using AirportWebsite.Core.Interfaces;
using Microsoft.EntityFrameworkCore;
using AirportWebsite.Data.Context;

namespace AirportWebsite.Data.Repositories;

public class BookingRepository : IBookingRepository
{
    private readonly ApplicationDbContext _context;

    public BookingRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Booking?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _context.Bookings
            .Include(b => b.Flight)
            .Include(b => b.Passenger)
            .Include(b => b.Payment)
            .FirstOrDefaultAsync(b => b.Id == id && !b.IsDeleted, cancellationToken);
    }

    public async Task<Booking?> GetByReferenceAsync(string bookingReference, CancellationToken cancellationToken = default)
    {
        return await _context.Bookings
            .Include(b => b.Flight)
            .Include(b => b.Passenger)
            .Include(b => b.Payment)
            .FirstOrDefaultAsync(b => b.BookingReference == bookingReference && !b.IsDeleted, cancellationToken);
    }

    public async Task<IEnumerable<Booking>> GetByUserIdAsync(string userId, CancellationToken cancellationToken = default)
    {
        return await _context.Bookings
            .Include(b => b.Flight)
            .Include(b => b.Passenger)
            .Where(b => b.UserId == userId && !b.IsDeleted)
            .OrderByDescending(b => b.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task AddAsync(Booking booking, CancellationToken cancellationToken = default)
    {
        await _context.Bookings.AddAsync(booking, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateAsync(Booking booking, CancellationToken cancellationToken = default)
    {
        booking.UpdatedAt = DateTime.UtcNow;
        _context.Bookings.Update(booking);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task<IEnumerable<Booking>> GetExpiredPendingBookingsAsync(DateTime expiryTime, CancellationToken cancellationToken = default)
    {
        return await _context.Bookings
            .Include(b => b.Flight)
            .Where(b => b.Status == Core.Enums.BookingStatus.Pending 
                     && b.PaymentDeadline < expiryTime
                     && !b.IsDeleted)
            .ToListAsync(cancellationToken);
    }
}
