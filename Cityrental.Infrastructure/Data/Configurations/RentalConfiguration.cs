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
    public class RentalConfiguration : IEntityTypeConfiguration<Rental>
    {
        public void Configure(EntityTypeBuilder<Rental> builder)
        {
            builder.ToTable("Rentals");

            builder.HasKey(r => r.Id);

            builder.Property(r => r.PickupDate)
                .IsRequired();

            builder.Property(r => r.ReturnDate)
                .IsRequired();

            builder.Property(r => r.DailyRate)
                .IsRequired()
                .HasColumnType("decimal(18,2)");

            builder.Property(r => r.TotalAmount)
                .IsRequired()
                .HasColumnType("decimal(18,2)");

            builder.Property(r => r.DepositAmount)
                .IsRequired()
                .HasColumnType("decimal(18,2)");

            builder.Property(r => r.Notes)
                .HasMaxLength(500);

            builder.HasIndex(r => r.Status);
            builder.HasIndex(r => r.PickupDate);
            builder.HasIndex(r => r.ReturnDate);

            // Relationships
            builder.HasOne(r => r.PickupLocation)
                .WithMany(l => l.RentalsAsPickup)
                .HasForeignKey(r => r.PickupLocationId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(r => r.ReturnLocation)
                .WithMany(l => l.RentalsAsReturn)
                .HasForeignKey(r => r.ReturnLocationId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(r => r.Payment)
                .WithOne(p => p.Rental)
                .HasForeignKey<Payment>(p => p.RentalId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(r => r.Review)
                .WithOne(rev => rev.Rental)
                .HasForeignKey<Review>(rev => rev.RentalId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
