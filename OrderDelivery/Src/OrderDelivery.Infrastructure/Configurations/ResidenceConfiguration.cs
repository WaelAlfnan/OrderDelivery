using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OrderDelivery.Domain.Entities;

namespace OrderDelivery.Infrastructure.Configurations
{
    public class ResidenceConfiguration : IEntityTypeConfiguration<Residence>
    {
        public void Configure(EntityTypeBuilder<Residence> builder)
        {
            // Table name
            builder.ToTable("Residences");

            // Primary Key
            builder.HasKey(x => x.Id);

            // Foreign Key
            builder.Property(x => x.DriverId)
                .IsRequired();

            // Properties configuration
            builder.Property(x => x.Province)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(x => x.City)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(x => x.District)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(x => x.Street)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(x => x.BuildingNumber)
                .HasMaxLength(50);

            builder.Property(x => x.Latitude)
                .IsRequired()
                .HasPrecision(18, 6);

            builder.Property(x => x.Longitude)
                .IsRequired()
                .HasPrecision(18, 6);

            // Relationships
            builder.HasOne(x => x.Driver)
                .WithOne(x => x.Residence)
                .HasForeignKey<Residence>(x => x.DriverId)
                .OnDelete(DeleteBehavior.Cascade);

            // Indexes
            builder.HasIndex(x => x.DriverId)
                .IsUnique();
        }
    }
} 