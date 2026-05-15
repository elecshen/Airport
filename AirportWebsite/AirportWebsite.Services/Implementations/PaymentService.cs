using AirportWebsite.Core.Entities;
using AirportWebsite.Core.Enums;
using AirportWebsite.Core.Exceptions;
using AirportWebsite.Core.Interfaces;
using Microsoft.Extensions.Logging;

namespace AirportWebsite.Services.Implementations;

public class PaymentService : IPaymentService
{
    private readonly ILogger<PaymentService> _logger;

    public PaymentService(ILogger<PaymentService> logger)
    {
        _logger = logger;
    }

    /// <summary>
    /// Processes payment with simulated gateway.
    /// Card "4111111111111111" always fails - for testing purposes.
    /// </summary>
    public async Task<(bool success, string transactionId, string lastFourDigits)> ProcessPaymentAsync(
        string cardNumber, 
        decimal amount, 
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Processing payment: Amount={Amount}, Card=****{LastFour}", 
            amount, cardNumber.Length >= 4 ? cardNumber[^4..] : "????");

        // Simulate network delay
        await Task.Delay(500, cancellationToken);

        // Test card that always fails
        if (cardNumber == "4111111111111111")
        {
            _logger.LogWarning("Payment declined for test card");
            throw new PaymentFailedException("Payment was declined by the bank. Please use a different card.");
        }

        // Validate card number format (basic Luhn check simulation)
        if (string.IsNullOrWhiteSpace(cardNumber) || cardNumber.Length < 13)
        {
            throw new PaymentFailedException("Invalid card number format.");
        }

        // Generate fake transaction ID
        var transactionId = $"TXN-{DateTime.UtcNow:yyyyMMddHHmmss}-{Guid.NewGuid().ToString("N")[..8]}";
        var lastFourDigits = cardNumber[^4..];

        _logger.LogInformation("Payment successful: TransactionId={TransactionId}", transactionId);

        return (true, transactionId, lastFourDigits);
    }
}
