using System;
using System.Linq;
using System.Collections.Generic;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Data
{
    public class NadafaDbContext : DbContext
    {
        public NadafaDbContext(DbContextOptions<NadafaDbContext> options) : base(options)
        {
        }

        // All entities
        public DbSet<User> Users { get; set; }
        public DbSet<Factory> Factories { get; set; }
        public DbSet<Payment> Payments { get; set; }
        public DbSet<PickupRequest> PickupRequests { get; set; }
        public DbSet<MarketplaceItem> MarketplaceItems { get; set; }
        public DbSet<Purchase> Purchases { get; set; }
        public DbSet<Notification> Notifications { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure User entity
            modelBuilder.Entity<User>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Email).IsRequired().HasMaxLength(150);
                entity.HasIndex(e => e.Email).IsUnique();
                entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Address).IsRequired().HasMaxLength(500);
                entity.Property(e => e.Age).IsRequired();
                entity.Property(e => e.PasswordHash).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Role).IsRequired();
                entity.Property(e => e.CreatedAt).IsRequired();
                entity.Property(e => e.IsActive).IsRequired();
            });

            // Configure Factory entity
            modelBuilder.Entity<Factory>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Email).IsRequired().HasMaxLength(150);
                entity.HasIndex(e => e.Email).IsUnique();
                entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Address).IsRequired().HasMaxLength(500);
                entity.Property(e => e.PasswordHash).IsRequired().HasMaxLength(100);
                entity.Property(e => e.PhoneNumber).HasMaxLength(20);
                entity.Property(e => e.BusinessLicense).HasMaxLength(100);
                entity.Property(e => e.Role).IsRequired();
                entity.Property(e => e.CreatedAt).IsRequired();
                entity.Property(e => e.IsActive).IsRequired();
                entity.Property(e => e.IsVerified).IsRequired();
            });

            // Configure Payment entity
            modelBuilder.Entity<Payment>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.StripePaymentIntentId).IsRequired().HasMaxLength(100);
                entity.Property(e => e.StripeCustomerId).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Amount).IsRequired().HasColumnType("decimal(18,2)");
                entity.Property(e => e.Currency).IsRequired().HasMaxLength(3);
                entity.Property(e => e.Status).IsRequired();
                entity.Property(e => e.Type).IsRequired();
                entity.Property(e => e.CreatedAt).IsRequired();
                entity.Property(e => e.Description).HasMaxLength(500);
                entity.Property(e => e.StripeResponse).HasMaxLength(1000);

                entity.HasOne(e => e.User)
                    .WithMany(u => u.Payments)
                    .HasForeignKey(e => e.UserId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(e => e.Factory)
                    .WithMany(f => f.Payments)
                    .HasForeignKey(e => e.FactoryId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // Configure PickupRequest entity
            modelBuilder.Entity<PickupRequest>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.MaterialType).IsRequired();
                entity.Property(e => e.Quantity).IsRequired().HasColumnType("decimal(18,2)");
                entity.Property(e => e.Unit).IsRequired();
                entity.Property(e => e.ProposedPricePerUnit).IsRequired().HasColumnType("decimal(18,2)");
                entity.Property(e => e.Description).IsRequired().HasMaxLength(500);
                entity.Property(e => e.Status).IsRequired();
                entity.Property(e => e.RequestDate).IsRequired();
                entity.Property(e => e.AdminNotes).HasMaxLength(1000);
                entity.Property(e => e.CreatedAt).IsRequired();
                entity.Property(e => e.UpdatedAt).IsRequired();

                // Configure ImageUrls as JSON with value comparer
                entity.Property(e => e.ImageUrls)
                    .HasConversion(
                        v => string.Join(',', v),
                        v => v.Split(',', StringSplitOptions.RemoveEmptyEntries).ToList()
                    )
                    .Metadata.SetValueComparer(new Microsoft.EntityFrameworkCore.ChangeTracking.ValueComparer<List<string>>(
                        (c1, c2) => c1.SequenceEqual(c2),
                        c => c.Aggregate(0, (a, v) => HashCode.Combine(a, v.GetHashCode())),
                        c => c.ToList()));

                entity.HasOne(e => e.User)
                    .WithMany(u => u.PickupRequests)
                    .HasForeignKey(e => e.UserId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(e => e.Admin)
                    .WithMany(u => u.AdminApprovedRequests)
                    .HasForeignKey(e => e.AdminId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // Configure MarketplaceItem entity
            modelBuilder.Entity<MarketplaceItem>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.MaterialType).IsRequired();
                entity.Property(e => e.Quantity).IsRequired().HasColumnType("decimal(18,2)");
                entity.Property(e => e.Unit).IsRequired();
                entity.Property(e => e.PricePerUnit).IsRequired().HasColumnType("decimal(18,2)");
                entity.Property(e => e.Description).IsRequired().HasMaxLength(500);
                entity.Property(e => e.IsAvailable).IsRequired();
                entity.Property(e => e.PublishedAt).IsRequired();
                entity.Property(e => e.CreatedAt).IsRequired();
                entity.Property(e => e.UpdatedAt).IsRequired();

                // Configure ImageUrls as JSON with value comparer
                entity.Property(e => e.ImageUrls)
                    .HasConversion(
                        v => string.Join(',', v),
                        v => v.Split(',', StringSplitOptions.RemoveEmptyEntries).ToList()
                    )
                    .Metadata.SetValueComparer(new Microsoft.EntityFrameworkCore.ChangeTracking.ValueComparer<List<string>>(
                        (c1, c2) => c1.SequenceEqual(c2),
                        c => c.Aggregate(0, (a, v) => HashCode.Combine(a, v.GetHashCode())),
                        c => c.ToList()));

                entity.HasOne(e => e.PickupRequest)
                    .WithOne(pr => pr.MarketplaceItem)
                    .HasForeignKey<MarketplaceItem>(e => e.PickupRequestId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(e => e.User)
                    .WithMany(u => u.MarketplaceItems)
                    .HasForeignKey(e => e.UserId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // Configure Purchase entity
            modelBuilder.Entity<Purchase>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Quantity).IsRequired().HasColumnType("decimal(18,2)");
                entity.Property(e => e.PricePerUnit).IsRequired().HasColumnType("decimal(18,2)");
                entity.Property(e => e.TotalAmount).IsRequired().HasColumnType("decimal(18,2)");
                entity.Property(e => e.StripePaymentIntentId).IsRequired().HasMaxLength(100);
                entity.Property(e => e.PaymentStatus).IsRequired();
                entity.Property(e => e.PurchaseDate).IsRequired();
                entity.Property(e => e.CreatedAt).IsRequired();
                entity.Property(e => e.UpdatedAt).IsRequired();

                entity.HasOne(e => e.MarketplaceItem)
                    .WithOne(mi => mi.Purchase)
                    .HasForeignKey<Purchase>(e => e.MarketplaceItemId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(e => e.Factory)
                    .WithMany(f => f.Purchases)
                    .HasForeignKey(e => e.FactoryId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // Configure Notification entity
            modelBuilder.Entity<Notification>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Title).IsRequired().HasMaxLength(200);
                entity.Property(e => e.Message).IsRequired().HasMaxLength(1000);
                entity.Property(e => e.NotificationType).IsRequired();
                entity.Property(e => e.IsRead).IsRequired();
                entity.Property(e => e.CreatedAt).IsRequired();

                entity.HasOne(e => e.User)
                    .WithMany(u => u.Notifications)
                    .HasForeignKey(e => e.UserId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // Seed initial admin user
            modelBuilder.Entity<User>().HasData(new User
            {
                Id = 1,
                Name = "Admin User",
                Email = "admin@nadafa.com",
                Address = "Admin Address",
                Age = 30,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("Admin123!"),
                Role = Role.Admin,
                CreatedAt = DateTime.UtcNow,
                IsActive = true
            });
        }
    }
}
