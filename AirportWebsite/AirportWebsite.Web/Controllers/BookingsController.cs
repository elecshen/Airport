using Microsoft.AspNetCore.Mvc;
using AirportWebsite.Core.Interfaces;
using AirportWebsite.Web.ViewModels;
using Microsoft.AspNetCore.Authorization;

namespace AirportWebsite.Web.Controllers;

public class BookingsController : Controller
{
    private readonly IFlightsService _flightsService;
    private readonly IBookingService _bookingService;
    private readonly IPassengerService _passengerService;
    private readonly ILogger<BookingsController> _logger;

    public BookingsController(
        IFlightsService flightsService,
        IBookingService bookingService,
        IPassengerService passengerService,
        ILogger<BookingsController> logger)
    {
        _flightsService = flightsService;
        _bookingService = bookingService;
        _passengerService = passengerService;
        _logger = logger;
    }

    [Authorize]
    [HttpGet]
    public async Task<IActionResult> Create(int id, CancellationToken cancellationToken)
    {
        var flight = await _flightsService.GetFlightByIdAsync(id, cancellationToken);
        if (flight == null)
        {
            return NotFound();
        }

        if (flight.AvailableSeats <= 0)
        {
            TempData["Error"] = "К сожалению, места на этом рейсе закончились.";
            return RedirectToAction(nameof(FlightsController.Index), "Flights");
        }

        var viewModel = new CreateBookingViewModel
        {
            FlightId = flight.Id,
            TotalPrice = flight.BasePrice,
            FlightInfo = $"{flight.Origin} → {flight.Destination} ({flight.FlightNumber})"
        };

        return View(viewModel);
    }

    [Authorize]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CreateBookingViewModel model, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            var flight = await _flightsService.GetFlightByIdAsync(model.FlightId, cancellationToken);
            if (flight != null)
                model.FlightInfo = $"{flight.Origin} → {flight.Destination} ({flight.FlightNumber})";
            
            return View(model);
        }

        var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userId))
        {
            return Challenge();
        }

        try
        {
            // Create or find passenger
            var passenger = await _passengerService.CreateOrFindPassengerAsync(
                model.FirstName,
                model.LastName,
                model.Email,
                model.PassportNumber,
                model.PassportCountry,
                model.DateOfBirth,
                model.PhoneNumber,
                cancellationToken
            );

            // Create booking
            var (booking, bookingReference) = await _bookingService.CreateBookingAsync(
                model.FlightId,
                passenger.Id,
                model.PassengerCount,
                userId,
                cancellationToken
            );

            // Process payment
            var updatedBooking = await _bookingService.ProcessPaymentAsync(
                bookingReference,
                model.CardNumber,
                cancellationToken
            );

            TempData["Success"] = $"Бронирование успешно! Номер заказа: {updatedBooking.BookingReference}";
            return RedirectToAction(nameof(MyBookings));
        }
        catch (Core.Exceptions.PaymentFailedException ex)
        {
            ModelState.AddModelError(string.Empty, ex.Message);
            var flight = await _flightsService.GetFlightByIdAsync(model.FlightId, cancellationToken);
            if (flight != null)
                model.FlightInfo = $"{flight.Origin} → {flight.Destination} ({flight.FlightNumber})";
            
            return View(model);
        }
        catch (Core.Exceptions.InsufficientSeatsException ex)
        {
            ModelState.AddModelError(string.Empty, ex.Message);
            var flight = await _flightsService.GetFlightByIdAsync(model.FlightId, cancellationToken);
            if (flight != null)
                model.FlightInfo = $"{flight.Origin} → {flight.Destination} ({flight.FlightNumber})";
            
            return View(model);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Ошибка при создании бронирования");
            ModelState.AddModelError(string.Empty, "Произошла ошибка при обработке вашего запроса. Пожалуйста, попробуйте позже.");
            var flight = await _flightsService.GetFlightByIdAsync(model.FlightId, cancellationToken);
            if (flight != null)
                model.FlightInfo = $"{flight.Origin} → {flight.Destination} ({flight.FlightNumber})";
            
            return View(model);
        }
    }

    [Authorize]
    [HttpGet]
    public async Task<IActionResult> MyBookings(CancellationToken cancellationToken)
    {
        var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userId))
        {
            return Challenge();
        }

        var bookings = await _bookingService.GetUserBookingsAsync(userId, cancellationToken);
        
        var viewModel = bookings.Select(b => new UserBookingsViewModel
        {
            Id = b.Id,
            BookingReference = b.BookingReference,
            FlightNumber = b.Flight.FlightNumber,
            Origin = b.Flight.Origin,
            Destination = b.Flight.Destination,
            DepartureTime = b.Flight.DepartureTime,
            Status = b.Status.ToString(),
            PassengerName = $"{b.Passenger.FirstName} {b.Passenger.LastName}",
            TotalPrice = b.TotalPrice
        }).ToList();

        return View(viewModel);
    }
}
