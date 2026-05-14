using AirportWebsite.Core.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace AirportWebsite.Data.Context;

public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<Flight> Flights => Set<Flight>();
    public DbSet<Passenger> Passengers => Set<Passenger>();
    public DbSet<Booking> Bookings => Set<Booking>();
    public DbSet<Payment> Payments => Set<Payment>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        // Flight configuration
        builder.Entity<Flight>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.FlightNumber);
            entity.HasIndex(e => new { e.Origin, e.Destination, e.DepartureTime });
            entity.Property(e => e.RowVersion).IsRowVersion();
        });

        // Passenger configuration
        builder.Entity<Passenger>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => new { e.PassportNumber, e.PassportCountry })
                  .IsUnique();
            entity.Property(e => e.RowVersion).IsRowVersion();
        });

        // Booking configuration
        builder.Entity<Booking>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.BookingReference).IsUnique();
            entity.HasIndex(e => e.UserId);
            entity.HasOne(e => e.Flight)
                  .WithMany(f => f.Bookings)
                  .HasForeignKey(e => e.FlightId)
                  .OnDelete(DeleteBehavior.Restrict);
            entity.HasOne(e => e.Passenger)
                  .WithMany(p => p.Bookings)
                  .HasForeignKey(e => e.PassengerId)
                  .OnDelete(DeleteBehavior.Restrict);
            entity.Property(e => e.RowVersion).IsRowVersion();
        });

        // Payment configuration
        builder.Entity<Payment>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.BookingId).IsUnique();
            entity.HasOne(e => e.Booking)
                  .WithOne(b => b.Payment)
                  .HasForeignKey<Payment>(e => e.BookingId)
                  .OnDelete(DeleteBehavior.Cascade);
            entity.Property(e => e.RowVersion).IsRowVersion();
        });

        // ApplicationUser configuration
        builder.Entity<ApplicationUser>(entity =>
        {
            entity.ToTable("AspNetUsers");
        });
    }
}
