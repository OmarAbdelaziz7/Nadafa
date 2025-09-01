using System;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Data
{
    public class NadafaDbContext : DbContext
    {
        public NadafaDbContext(DbContextOptions<NadafaDbContext> options) : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Factory> Factories { get; set; }
        public DbSet<PickupRequest> PickupRequests { get; set; }
        public DbSet<MarketplaceItem> MarketplaceItems { get; set; }
        public DbSet<Payment> Payments { get; set; }

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

            // Configure PickupRequest entity
            modelBuilder.Entity<PickupRequest>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.MaterialType).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Description).IsRequired().HasMaxLength(500);
                entity.Property(e => e.Price).IsRequired().HasColumnType("decimal(18,2)");
                entity.Property(e => e.Weight).IsRequired().HasColumnType("decimal(18,2)");
                entity.Property(e => e.WeightUnit).IsRequired().HasMaxLength(20);
                entity.Property(e => e.RequestDate).IsRequired();
                entity.Property(e => e.Status).IsRequired();
                entity.Property(e => e.AdminNotes).HasMaxLength(1000);

                entity.HasOne(e => e.User)
                    .WithMany(u => u.PickupRequests)
                    .HasForeignKey(e => e.UserId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(e => e.ApprovedByAdmin)
                    .WithMany()
                    .HasForeignKey(e => e.ApprovedByAdminId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // Configure MarketplaceItem entity
            modelBuilder.Entity<MarketplaceItem>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.MaterialType).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Description).IsRequired().HasMaxLength(500);
                entity.Property(e => e.Price).IsRequired().HasColumnType("decimal(18,2)");
                entity.Property(e => e.Weight).IsRequired().HasColumnType("decimal(18,2)");
                entity.Property(e => e.WeightUnit).IsRequired().HasMaxLength(20);
                entity.Property(e => e.PublishedDate).IsRequired();
                entity.Property(e => e.Status).IsRequired();
                entity.Property(e => e.PurchasePrice).HasColumnType("decimal(18,2)");
                entity.Property(e => e.StripePaymentIntentId).HasMaxLength(100);

                entity.HasOne(e => e.PickupRequest)
                    .WithOne(pr => pr.MarketplaceItem)
                    .HasForeignKey<MarketplaceItem>(e => e.PickupRequestId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(e => e.PurchasedByFactory)
                    .WithMany(f => f.PurchasedItems)
                    .HasForeignKey(e => e.PurchasedByFactoryId)
                    .OnDelete(DeleteBehavior.Restrict);
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
