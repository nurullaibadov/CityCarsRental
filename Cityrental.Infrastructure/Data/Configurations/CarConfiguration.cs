using Cityrental.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cityrental.Infrastructure.Data.Configurations
{
    public class CarConfiguration : IEntityTypeConfiguration<Car>
    {
        public void Configure(EntityTypeBuilder<Car> builder)
        {
            builder.ToTable("Cars");

            builder.HasKey(c => c.Id);

            builder.Property(c => c.Model)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(c => c.Color)
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(c => c.LicensePlate)
                .IsRequired()
                .HasMaxLength(20);

            builder.HasIndex(c => c.LicensePlate)
                .IsUnique();

            builder.Property(c => c.VIN)
                .HasMaxLength(17);

            builder.HasIndex(c => c.VIN)
                .IsUnique();

            builder.Property(c => c.DailyRate)
                .IsRequired()
                .HasColumnType("decimal(18,2)");

            builder.Property(c => c.Features)
                .HasMaxLength(1000);

            builder.Property(c => c.Description)
                .HasMaxLength(1000);

            builder.Property(c => c.MainImageUrl)
                .HasMaxLength(500);

            builder.Property(c => c.ImageUrls)
                .HasMaxLength(2000);

            builder.HasIndex(c => c.Status);
            builder.HasIndex(c => c.DailyRate);

            // Relationships
            builder.HasOne(c => c.Brand)
                .WithMany(b => b.Cars)
                .HasForeignKey(c => c.BrandId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(c => c.Category)
                .WithMany(cat => cat.Cars)
                .HasForeignKey(c => c.CategoryId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(c => c.Rentals)
                .WithOne(r => r.Car)
                .HasForeignKey(r => r.CarId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(c => c.Reviews)
                .WithOne(r => r.Car)
                .HasForeignKey(r => r.CarId)
                .OnDelete(DeleteBehavior.Cascade);

            // Ignore computed properties
            builder.Ignore(c => c.AverageRating);
        }
    }
}
