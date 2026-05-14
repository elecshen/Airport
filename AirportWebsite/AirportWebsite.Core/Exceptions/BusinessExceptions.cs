namespace AirportWebsite.Core.Exceptions;

public class BusinessException : Exception
{
    public BusinessException(string message) : base(message) { }
    public BusinessException(string message, Exception innerException) : base(message, innerException) { }
}

public class FlightNotFoundException : BusinessException
{
    public FlightNotFoundException(int flightId) : base($"Flight with ID {flightId} not found") { }
    public FlightNotFoundException(string flightNumber) : base($"Flight with number {flightNumber} not found") { }
}

public class InsufficientSeatsException : BusinessException
{
    public InsufficientSeatsException(int available, int requested) 
        : base($"Insufficient seats. Available: {available}, Requested: {requested}") { }
}

public class PaymentFailedException : BusinessException
{
    public PaymentFailedException(string message) : base(message) { }
}

public class BookingExpiredException : BusinessException
{
    public BookingExpiredException(string bookingReference) 
        : base($"Booking {bookingReference} has expired due to payment timeout") { }
}
