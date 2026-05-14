using AirportWebsite.Core.Entities;

namespace AirportWebsite.Core.Interfaces;

public interface IFlightRepository
{
    Task<IEnumerable<Flight>> SearchFlightsAsync(string? origin, string? destination, DateTime? departureDate, CancellationToken cancellationToken = default);
    Task<Flight?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<Flight?> GetByFlightNumberAsync(string flightNumber, CancellationToken cancellationToken = default);
    Task AddAsync(Flight flight, CancellationToken cancellationToken = default);
    Task UpdateAsync(Flight flight, CancellationToken cancellationToken = default);
    Task<bool> DecreaseAvailableSeatsAsync(int flightId, int seatsCount, CancellationToken cancellationToken = default);
}

public interface IPassengerRepository
{
    Task<Passenger?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<Passenger?> FindByPassportAsync(string passportNumber, string passportCountry, CancellationToken cancellationToken = default);
    Task AddAsync(Passenger passenger, CancellationToken cancellationToken = default);
    Task UpdateAsync(Passenger passenger, CancellationToken cancellationToken = default);
}

public interface IBookingRepository
{
    Task<Booking?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<Booking?> GetByReferenceAsync(string bookingReference, CancellationToken cancellationToken = default);
    Task<IEnumerable<Booking>> GetByUserIdAsync(string userId, CancellationToken cancellationToken = default);
    Task AddAsync(Booking booking, CancellationToken cancellationToken = default);
    Task UpdateAsync(Booking booking, CancellationToken cancellationToken = default);
    Task<IEnumerable<Booking>> GetExpiredPendingBookingsAsync(DateTime expiryTime, CancellationToken cancellationToken = default);
}

public interface IPaymentRepository
{
    Task<Payment?> GetByBookingIdAsync(int bookingId, CancellationToken cancellationToken = default);
    Task AddAsync(Payment payment, CancellationToken cancellationToken = default);
    Task UpdateAsync(Payment payment, CancellationToken cancellationToken = default);
}
