using AirportWebsite.Core.Entities;

namespace AirportWebsite.Core.Interfaces;

public interface IFlightsService
{
    Task<IEnumerable<Flight>> SearchFlightsAsync(string? origin, string? destination, DateTime? departureDate, CancellationToken cancellationToken = default);
    Task<Flight?> GetFlightByIdAsync(int id, CancellationToken cancellationToken = default);
}

public interface IBookingService
{
    Task<(Booking booking, string bookingReference)> CreateBookingAsync(int flightId, int passengerId, int seatsCount, string? userId, CancellationToken cancellationToken = default);
    Task<Booking> ProcessPaymentAsync(string bookingReference, string cardNumber, CancellationToken cancellationToken = default);
    Task<Booking> CancelBookingAsync(string bookingReference, CancellationToken cancellationToken = default);
    Task<IEnumerable<Booking>> GetUserBookingsAsync(string userId, CancellationToken cancellationToken = default);
    Task ExpirePendingBookingsAsync(CancellationToken cancellationToken = default);
}

public interface IPaymentService
{
    Task<(bool success, string transactionId, string lastFourDigits)> ProcessPaymentAsync(string cardNumber, decimal amount, CancellationToken cancellationToken = default);
}

public interface IPassengerService
{
    Task<Passenger> CreateOrFindPassengerAsync(string firstName, string lastName, string email, string passportNumber, string passportCountry, DateTime? dateOfBirth, string? phoneNumber, CancellationToken cancellationToken = default);
    Task<Passenger?> GetPassengerByIdAsync(int id, CancellationToken cancellationToken = default);
}
