using Microsoft.EntityFrameworkCore;
using Mottu.Api.Entities;
using Mottu.Api.Enums;

namespace Mottu.Api.Data
{
    public class MottuDbContext : DbContext
    {
        public MottuDbContext(DbContextOptions<MottuDbContext> options) : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Motorcycle> Motorcycles { get; set; }
        public DbSet<DeliveryPerson> DeliveryPeople { get; set; }
        public DbSet<Rental> Rentals { get; set; }
        public DbSet<MotorcycleNotification> MotorcycleNotifications { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>()
                .HasIndex(u => u.Username)
                .IsUnique();

            modelBuilder.Entity<User>()
                .HasIndex(u => u.Email)
                .IsUnique();

            modelBuilder.Entity<Motorcycle>()
                .HasIndex(m => m.Plate)
                .IsUnique();

            modelBuilder.Entity<DeliveryPerson>()
                .HasIndex(dp => dp.Cnpj)
                .IsUnique();

            modelBuilder.Entity<DeliveryPerson>()
                .HasIndex(dp => dp.CnhNumber)
                .IsUnique();

            modelBuilder.Entity<DeliveryPerson>()
                .HasIndex(dp => dp.Identifier)
                .IsUnique();

            modelBuilder.Entity<DeliveryPerson>()
                .Property(dp => dp.CnhType)
                .HasConversion<string>();
        }
    }
}
