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
    public class LocationConfiguration : IEntityTypeConfiguration<Location>
    {
        public void Configure(EntityTypeBuilder<Location> builder)
        {
            builder.ToTable("Locations");

            builder.HasKey(l => l.Id);

            builder.Property(l => l.Name)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(l => l.Address)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(l => l.City)
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(l => l.Country)
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(l => l.Latitude)
                .HasColumnType("decimal(10,7)");

            builder.Property(l => l.Longitude)
                .HasColumnType("decimal(10,7)");
        }
    }
}
