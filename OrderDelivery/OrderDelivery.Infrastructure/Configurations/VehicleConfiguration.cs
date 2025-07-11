using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OrderDelivery.Domain.Entities;

namespace OrderDelivery.Infrastructure.Configurations
{
    public class VehicleConfiguration : IEntityTypeConfiguration<Vehicle>
    {
        public void Configure(EntityTypeBuilder<Vehicle> builder)
        {
            // Table name
            builder.ToTable("Vehicles");

            // Primary Key
            builder.HasKey(x => x.Id);

            // Foreign Key
            builder.Property(x => x.DriverId)
                .IsRequired();

            // Properties configuration
            builder.Property(x => x.VehicleBrand)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(x => x.ShasehPhotoUrl)
                .IsRequired()
                .HasMaxLength(500);

            builder.Property(x => x.EfragPhotoUrl)
                .IsRequired()
                .HasMaxLength(500);

            builder.Property(x => x.VehicleIssueDate)
                .IsRequired();

            builder.Property(x => x.VehiclePlateNumber)
                .HasMaxLength(20);

            // Relationships
            builder.HasOne(x => x.Driver)
                .WithOne(x => x.Vehicle)
                .HasForeignKey<Vehicle>(x => x.DriverId)
                .OnDelete(DeleteBehavior.Cascade);

            // Indexes
            builder.HasIndex(x => x.DriverId)
                .IsUnique();

        }
    }
} 