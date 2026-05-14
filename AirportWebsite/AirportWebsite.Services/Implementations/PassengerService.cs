using AirportWebsite.Core.Entities;
using AirportWebsite.Core.Enums;
using AirportWebsite.Core.Exceptions;
using AirportWebsite.Core.Interfaces;
using Microsoft.Extensions.Logging;

namespace AirportWebsite.Services.Implementations;

public class PassengerService : IPassengerService
{
    private readonly IPassengerRepository _passengerRepository;
    private readonly ILogger<PassengerService> _logger;

    public PassengerService(
        IPassengerRepository passengerRepository,
        ILogger<PassengerService> logger)
    {
        _passengerRepository = passengerRepository;
        _logger = logger;
    }

    public async Task<Passenger> CreateOrFindPassengerAsync(
        string firstName, 
        string lastName, 
        string email, 
        string passportNumber, 
        string passportCountry, 
        DateTime? dateOfBirth, 
        string? phoneNumber,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Creating or finding passenger: {FirstName} {LastName}, Passport: {PassportNumber}/{PassportCountry}", 
            firstName, lastName, passportNumber, passportCountry);

        // Try to find existing passenger by passport
        var existing = await _passengerRepository.FindByPassportAsync(passportNumber, passportCountry, cancellationToken);
        
        if (existing != null)
        {
            _logger.LogInformation("Found existing passenger with ID {Id}", existing.Id);
            return existing;
        }

        // Create new passenger
        var passenger = new Passenger
        {
            FirstName = firstName,
            LastName = lastName,
            Email = email,
            PassportNumber = passportNumber,
            PassportCountry = passportCountry,
            DateOfBirth = dateOfBirth,
            PhoneNumber = phoneNumber
        };

        await _passengerRepository.AddAsync(passenger, cancellationToken);
        _logger.LogInformation("Created new passenger with ID {Id}", passenger.Id);

        return passenger;
    }

    public async Task<Passenger?> GetPassengerByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _passengerRepository.GetByIdAsync(id, cancellationToken);
    }
}
