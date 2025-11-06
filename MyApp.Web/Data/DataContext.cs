using Microsoft.EntityFrameworkCore;
using MyApp.Web.Data.Entities;
using MyApp.Web.Data.Entities.Bookings;
using MyApp.Web.Data.Entities.Venues;
using MyApp.Web.Data.Entities.Package;

namespace MyApp.Web.Data
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options)
        { }

        public DbSet<User> Users { get; set; } = null!;
        public DbSet<Venue> Venues { get; set; } = null!;
        public DbSet<VenueImage> VenueImages { get; set; } = null!;
        public DbSet<AdditionalService> AdditionalServices { get; set; } = null!;

        // Package related DbSets
        public DbSet<VenuePackage> VenuePackages { get; set; } = null!;
        public DbSet<PackageService> PackageServices { get; set; } = null!;

        // Booking related DbSets
        public DbSet<Booking> Bookings { get; set; } = null!;
        public DbSet<BookingAdditionalService> BookingAdditionalServices { get; set; } = null!;
        public DbSet<BookingPackageService> BookingPackageServices { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // User-Booking (One-to-Many)
            modelBuilder.Entity<Booking>()
                .HasOne(b => b.User)
                .WithMany(u => u.Bookings)
                .HasForeignKey(b => b.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            // Venue-Booking (One-to-Many)
            modelBuilder.Entity<Booking>()
                .HasOne(b => b.Venue)
                .WithMany(v => v.Bookings)
                .HasForeignKey(b => b.VenueId)
                .OnDelete(DeleteBehavior.Restrict);

            // Venue-VenueImage (One-to-Many)
            modelBuilder.Entity<VenueImage>()
                .HasOne(vi => vi.Venue)
                .WithMany(v => v.Images)
                .HasForeignKey(vi => vi.VenueId)
                .OnDelete(DeleteBehavior.Cascade);

            // Venue-AdditionalService (One-to-Many)
            modelBuilder.Entity<AdditionalService>()
                .HasOne(s => s.Venue)
                .WithMany(v => v.AdditionalServices)
                .HasForeignKey(s => s.VenueId)
                .OnDelete(DeleteBehavior.Cascade);

            // Venue-VenuePackage (One-to-Many)
            modelBuilder.Entity<VenuePackage>()
                .HasOne(vp => vp.Venue)
                .WithMany(v => v.Packages)
                .HasForeignKey(vp => vp.VenueId)
                .OnDelete(DeleteBehavior.Cascade);

            // VenuePackage-PackageService (One-to-Many)
            modelBuilder.Entity<PackageService>()
                .HasOne(ps => ps.Package)
                .WithMany(p => p.PackageServices)
                .HasForeignKey(ps => ps.PackageId)
                .OnDelete(DeleteBehavior.Cascade);

            // Booking-VenuePackage (Optional One-to-Many)
            modelBuilder.Entity<Booking>()
                .HasOne(b => b.Package)
                .WithMany()
                .HasForeignKey(b => b.PackageId)
                .OnDelete(DeleteBehavior.Restrict);

            // Booking-BookingAdditionalService (Many-to-Many)
            modelBuilder.Entity<BookingAdditionalService>()
                .HasKey(bas => bas.Id);

            modelBuilder.Entity<BookingAdditionalService>()
                .HasOne(bas => bas.Booking)
                .WithMany(b => b.BookingAdditionalServices)
                .HasForeignKey(bas => bas.BookingId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<BookingAdditionalService>()
                .HasOne(bas => bas.AdditionalService)
                .WithMany()
                .HasForeignKey(bas => bas.AdditionalServiceId)
                .OnDelete(DeleteBehavior.Restrict);

            // Booking-BookingPackageService (Many-to-Many)
            modelBuilder.Entity<BookingPackageService>()
                .HasKey(bps => bps.Id);

            modelBuilder.Entity<BookingPackageService>()
                .HasOne(bps => bps.Booking)
                .WithMany(b => b.BookingPackageServices)
                .HasForeignKey(bps => bps.BookingId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<BookingPackageService>()
                .HasOne(bps => bps.PackageService)
                .WithMany()
                .HasForeignKey(bps => bps.PackageServiceId)
                .OnDelete(DeleteBehavior.Restrict);

            // Configure decimal precision for remaining entities
            modelBuilder.Entity<AdditionalService>()
                .Property(a => a.Price)
                .HasPrecision(18, 2);

            modelBuilder.Entity<VenuePackage>()
                .Property(vp => vp.BasePrice)
                .HasPrecision(18, 2);

            modelBuilder.Entity<VenuePackage>()
                .Property(vp => vp.PricePerAdditionalPerson)
                .HasPrecision(18, 2);

            modelBuilder.Entity<VenuePackage>()
                .Property(vp => vp.PricePerAdditionalHour)
                .HasPrecision(18, 2);

            modelBuilder.Entity<PackageService>()
                .Property(ps => ps.Price)
                .HasPrecision(18, 2);

            modelBuilder.Entity<Booking>()
                .Property(b => b.BasePackageCost)
                .HasPrecision(18, 2);

            modelBuilder.Entity<Booking>()
                .Property(b => b.CapacityExtensionCost)
                .HasPrecision(18, 2);

            modelBuilder.Entity<Booking>()
                .Property(b => b.DurationExtensionCost)
                .HasPrecision(18, 2);

            modelBuilder.Entity<Booking>()
                .Property(b => b.AdditionalServicesCost)
                .HasPrecision(18, 2);

            modelBuilder.Entity<Booking>()
                .Property(b => b.TotalCost)
                .HasPrecision(18, 2);

            // Indexes for performance
            modelBuilder.Entity<Booking>()
                .HasIndex(b => b.VenueId);

            modelBuilder.Entity<Booking>()
                .HasIndex(b => new { b.StartTime, b.EndTime });

            modelBuilder.Entity<VenuePackage>()
                .HasIndex(vp => vp.VenueId);

            modelBuilder.Entity<AdditionalService>()
                .HasIndex(a => a.VenueId);
        }
    }
}