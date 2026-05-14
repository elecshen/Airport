using AirportWebsite.Core.Entities;
using AirportWebsite.Data.Context;
using Microsoft.EntityFrameworkCore;

namespace AirportWebsite.Data.SeedData;

public static class DbSeeder
{
    public static async Task SeedAsync(ApplicationDbContext context)
    {
        if (context.Flights.Any())
        {
            return; // Already seeded
        }

        var flights = new List<Flight>
        {
            new Flight
            {
                FlightNumber = "AW101",
                Origin = "Moscow",
                Destination = "Saint Petersburg",
                DepartureTime = DateTime.UtcNow.AddDays(1).AddHours(8),
                ArrivalTime = DateTime.UtcNow.AddDays(1).AddHours(10),
                Status = Core.Enums.FlightStatus.Scheduled,
                TotalSeats = 150,
                AvailableSeats = 120,
                BasePrice = 5000m,
                Gate = "A1",
                Terminal = "1"
            },
            new Flight
            {
                FlightNumber = "AW102",
                Origin = "Moscow",
                Destination = "Sochi",
                DepartureTime = DateTime.UtcNow.AddDays(1).AddHours(12),
                ArrivalTime = DateTime.UtcNow.AddDays(1).AddHours(15),
                Status = Core.Enums.FlightStatus.Scheduled,
                TotalSeats = 180,
                AvailableSeats = 45,
                BasePrice = 8500m,
                Gate = "B3",
                Terminal = "2"
            },
            new Flight
            {
                FlightNumber = "AW103",
                Origin = "Saint Petersburg",
                Destination = "Moscow",
                DepartureTime = DateTime.UtcNow.AddDays(2).AddHours(9),
                ArrivalTime = DateTime.UtcNow.AddDays(2).AddHours(11),
                Status = Core.Enums.FlightStatus.Scheduled,
                TotalSeats = 150,
                AvailableSeats = 89,
                BasePrice = 5200m,
                Gate = "C2",
                Terminal = "1"
            },
            new Flight
            {
                FlightNumber = "AW104",
                Origin = "Moscow",
                Destination = "Vladivostok",
                DepartureTime = DateTime.UtcNow.AddDays(3).AddHours(6),
                ArrivalTime = DateTime.UtcNow.AddDays(3).AddHours(14),
                Status = Core.Enums.FlightStatus.Scheduled,
                TotalSeats = 250,
                AvailableSeats = 200,
                BasePrice = 25000m,
                Gate = "D5",
                Terminal = "3"
            },
            new Flight
            {
                FlightNumber = "AW105",
                Origin = "Sochi",
                Destination = "Moscow",
                DepartureTime = DateTime.UtcNow.AddDays(1).AddHours(18),
                ArrivalTime = DateTime.UtcNow.AddDays(1).AddHours(21),
                Status = Core.Enums.FlightStatus.Boarding,
                TotalSeats = 180,
                AvailableSeats = 12,
                BasePrice = 8700m,
                Gate = "A5",
                Terminal = "2"
            }
        };

        await context.Flights.AddRangeAsync(flights);
        await context.SaveChangesAsync();
    }
}
