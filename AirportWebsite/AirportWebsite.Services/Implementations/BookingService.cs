using AirportWebsite.Core.Entities;
using AirportWebsite.Core.Enums;
using AirportWebsite.Core.Exceptions;
using AirportWebsite.Core.Interfaces;
using Microsoft.Extensions.Logging;

namespace AirportWebsite.Services.Implementations;

public class BookingService : IBookingService
{
    private readonly IFlightRepository _flightRepository;
    private readonly IPassengerRepository _passengerRepository;
    private readonly IBookingRepository _bookingRepository;
    private readonly IPaymentRepository _paymentRepository;
    private readonly IPaymentService _paymentService;
    private readonly ILogger<BookingService> _logger;
    private readonly TimeSpan PaymentTimeout = TimeSpan.FromMinutes(15);

    public BookingService(
        IFlightRepository flightRepository,
        IPassengerRepository passengerRepository,
        IBookingRepository bookingRepository,
        IPaymentRepository paymentRepository,
        IPaymentService paymentService,
        ILogger<BookingService> logger)
    {
        _flightRepository = flightRepository;
        _passengerRepository = passengerRepository;
        _bookingRepository = bookingRepository;
        _paymentRepository = paymentRepository;
        _paymentService = paymentService;
        _logger = logger;
    }

    public async Task<(Booking booking, string bookingReference)> CreateBookingAsync(
        int flightId, 
        int passengerId, 
        int seatsCount, 
        string? userId,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Creating booking: FlightId={FlightId}, PassengerId={PassengerId}, Seats={Seats}", 
            flightId, passengerId, seatsCount);

        // Get flight and check availability
        var flight = await _flightRepository.GetByIdAsync(flightId, cancellationToken)
            ?? throw new FlightNotFoundException(flightId);

        if (flight.AvailableSeats < seatsCount)
        {
            throw new InsufficientSeatsException(flight.AvailableSeats, seatsCount);
        }

        // Get passenger
        var passenger = await _passengerRepository.GetByIdAsync(passengerId, cancellationToken);
        if (passenger == null)
        {
            throw new BusinessException($"Passenger with ID {passengerId} not found");
        }

        // Calculate total price
        var totalPrice = flight.BasePrice * seatsCount;

        // Generate booking reference
        var bookingReference = GenerateBookingReference();

        // Create booking with Pending status
        var booking = new Booking
        {
            BookingReference = bookingReference,
            FlightId = flightId,
            PassengerId = passengerId,
            SeatsCount = seatsCount,
            TotalPrice = totalPrice,
            Status = BookingStatus.Pending,
            PaymentDeadline = DateTime.UtcNow.Add(PaymentTimeout),
            UserId = userId
        };

        await _bookingRepository.AddAsync(booking, cancellationToken);
        _logger.LogInformation("Created booking with reference {Reference}", bookingReference);

        return (booking, bookingReference);
    }

    public async Task<Booking> ProcessPaymentAsync(
        string bookingReference, 
        string cardNumber, 
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Processing payment for booking {Reference}", bookingReference);

        var booking = await _bookingRepository.GetByReferenceAsync(bookingReference, cancellationToken)
            ?? throw new BusinessException($"Booking {bookingReference} not found");

        // Check if booking is still pending
        if (booking.Status != BookingStatus.Pending)
        {
            throw new BusinessException($"Booking {bookingReference} is already {booking.Status}");
        }

        // Check if booking has expired
        if (booking.PaymentDeadline < DateTime.UtcNow)
        {
            booking.Status = BookingStatus.Expired;
            await _bookingRepository.UpdateAsync(booking, cancellationToken);
            throw new BookingExpiredException(bookingReference);
        }

        // Process payment through payment service
        var (success, transactionId, lastFourDigits) = await _paymentService.ProcessPaymentAsync(cardNumber, booking.TotalPrice, cancellationToken);

        // Create payment record
        var payment = new Payment
        {
            BookingId = booking.Id,
            TransactionId = transactionId,
            Amount = booking.TotalPrice,
            CardLastFourDigits = lastFourDigits,
            IsSuccess = success,
            AttemptCount = 1,
            LastAttemptAt = DateTime.UtcNow
        };

        await _paymentRepository.AddAsync(payment, cancellationToken);

        // Update booking status
        booking.Status = BookingStatus.Paid;
        booking.PaidAt = DateTime.UtcNow;
        await _bookingRepository.UpdateAsync(booking, cancellationToken);

        _logger.LogInformation("Payment successful for booking {Reference}, TransactionId={TransactionId}", 
            bookingReference, transactionId);

        return booking;
    }

    public async Task<Booking> CancelBookingAsync(string bookingReference, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Cancelling booking {Reference}", bookingReference);

        var booking = await _bookingRepository.GetByReferenceAsync(bookingReference, cancellationToken)
            ?? throw new BusinessException($"Booking {bookingReference} not found");

        if (booking.Status == BookingStatus.Cancelled)
        {
            throw new BusinessException($"Booking {bookingReference} is already cancelled");
        }

        if (booking.Status == BookingStatus.Completed)
        {
            throw new BusinessException($"Cannot cancel completed booking {bookingReference}");
        }

        // Restore seats if booking was paid
        if (booking.Status == BookingStatus.Paid && booking.Flight != null)
        {
            // Note: In production, you'd want to handle this in a transaction
            // and potentially have a separate method to restore seats
        }

        booking.Status = BookingStatus.Cancelled;
        await _bookingRepository.UpdateAsync(booking, cancellationToken);

        _logger.LogInformation("Booking {Reference} cancelled", bookingReference);

        return booking;
    }

    public async Task<IEnumerable<Booking>> GetUserBookingsAsync(string userId, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Getting bookings for user {UserId}", userId);
        return await _bookingRepository.GetByUserIdAsync(userId, cancellationToken);
    }

    public async Task ExpirePendingBookingsAsync(CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Expiring pending bookings older than {TimeoutMinutes} minutes", PaymentTimeout.TotalMinutes);

        var expiryTime = DateTime.UtcNow.Subtract(PaymentTimeout);
        var expiredBookings = await _bookingRepository.GetExpiredPendingBookingsAsync(expiryTime, cancellationToken);

        foreach (var booking in expiredBookings)
        {
            try
            {
                booking.Status = BookingStatus.Expired;
                await _bookingRepository.UpdateAsync(booking, cancellationToken);
                _logger.LogInformation("Expired booking {Reference}", booking.BookingReference);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to expire booking {Reference}", booking.BookingReference);
            }
        }

        _logger.LogInformation("Expired {Count} bookings", expiredBookings.Count());
    }

    private static string GenerateBookingReference()
    {
        return $"AW-{DateTime.UtcNow:yyyyMMdd}-{Guid.NewGuid().ToString("N")[..8]}";
    }
}
