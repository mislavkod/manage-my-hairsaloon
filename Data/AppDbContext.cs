using manage_my_hairsaloon.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace manage_my_hairsaloon.Data
{
    public class AppDbContext : IdentityDbContext<AppUser>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<User> BusinessUsers { get; set; }
        public DbSet<HairSalon> HairSalons { get; set; }
        public DbSet<Staff> Staff { get; set; }
        public DbSet<Service> Services { get; set; }
        public DbSet<Reservation> Reservations { get; set; }
        public DbSet<Review> Reviews { get; set; }
        public DbSet<SalonPhoto> SalonPhotos { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Decimal precision
            modelBuilder.Entity<Service>()
                .Property(s => s.Price)
                .HasPrecision(10, 2);

            modelBuilder.Entity<Staff>()
                .Property(s => s.HourlyRate)
                .HasPrecision(10, 2);

            // User → Staff (1-to-1)
            modelBuilder.Entity<Staff>()
                .HasOne(s => s.User)
                .WithOne(u => u.Staff)
                .HasForeignKey<Staff>(s => s.UserId);

            // Staff → HairSalon (many-to-1)
            modelBuilder.Entity<Staff>()
                .HasOne(s => s.HairSalon)
                .WithMany(h => h.Staff)
                .HasForeignKey(s => s.HairSalonId);

            // Service → HairSalon (many-to-1)
            modelBuilder.Entity<Service>()
                .HasOne(s => s.HairSalon)
                .WithMany(h => h.Services)
                .HasForeignKey(s => s.HairSalonId);

            // Reservation → Customer (User) — explicit FK to avoid ambiguity
            modelBuilder.Entity<Reservation>()
                .HasOne(r => r.Customer)
                .WithMany(u => u.CustomerReservations)
                .HasForeignKey(r => r.CustomerId)
                .OnDelete(DeleteBehavior.Restrict);

            // Reservation → Staff
            modelBuilder.Entity<Reservation>()
                .HasOne(r => r.Staff)
                .WithMany(s => s.Reservations)
                .HasForeignKey(r => r.StaffId)
                .OnDelete(DeleteBehavior.Restrict);

            // Reservation → Service
            modelBuilder.Entity<Reservation>()
                .HasOne(r => r.Service)
                .WithMany(s => s.Reservations)
                .HasForeignKey(r => r.ServiceId)
                .OnDelete(DeleteBehavior.Restrict);

            // Review → Reservation
            modelBuilder.Entity<Review>()
                .HasOne(r => r.Reservation)
                .WithMany(res => res.Reviews)
                .HasForeignKey(r => r.ReservationId)
                .OnDelete(DeleteBehavior.Cascade);

            // Review → Customer (User)
            modelBuilder.Entity<Review>()
                .HasOne(r => r.Customer)
                .WithMany(u => u.Reviews)
                .HasForeignKey(r => r.CustomerId)
                .OnDelete(DeleteBehavior.Restrict);

            // SalonPhoto → HairSalon
            modelBuilder.Entity<SalonPhoto>()
                .HasOne(p => p.HairSalon)
                .WithMany(h => h.Photos)
                .HasForeignKey(p => p.HairSalonId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
