using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OrderDelivery.Domain.Entities;

namespace OrderDelivery.Infrastructure.Configurations
{
    public class DriverConfiguration : IEntityTypeConfiguration<Driver>
    {
        public void Configure(EntityTypeBuilder<Driver> builder)
        {
            // Table name
            builder.ToTable("Drivers");

            // Primary Key
            builder.HasKey(x => x.Id);

            // Foreign Key
            builder.Property(x => x.ApplicationUserId)
                .IsRequired();

            // Properties configuration
            builder.Property(x => x.VehicleType)
                .IsRequired();

            builder.Property(x => x.IsAvailable)
                .HasDefaultValue(false);

            builder.Property(x => x.Rating)
                .HasPrecision(3, 2)
                .HasDefaultValue(0);

            builder.Property(x => x.TotalDeliveries)
                .HasDefaultValue(0);

            builder.Property(x => x.CurrentLatitude)
                .IsRequired()
                .HasPrecision(18, 6);

            builder.Property(x => x.CurrentLongitude)
                .IsRequired()
                .HasPrecision(18, 6);

            // Relationships
            builder.HasOne(x => x.AppUser)
                .WithOne(x => x.Driver)
                .HasForeignKey<Driver>(x => x.ApplicationUserId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(x => x.Vehicle)
                .WithOne(x => x.Driver)
                .HasForeignKey<Vehicle>(x => x.DriverId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(x => x.Residence)
                .WithOne(x => x.Driver)
                .HasForeignKey<Residence>(x => x.DriverId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(x => x.OrdersDelivered)
                .WithOne(x => x.Driver)
                .HasForeignKey(x => x.DriverId)
                .OnDelete(DeleteBehavior.Restrict);

            // Indexes
            builder.HasIndex(x => x.ApplicationUserId)
                .IsUnique();

            builder.HasIndex(x => new { x.CurrentLatitude, x.CurrentLongitude });
            
            builder.HasIndex(x => x.IsAvailable);
        }
    }
} 