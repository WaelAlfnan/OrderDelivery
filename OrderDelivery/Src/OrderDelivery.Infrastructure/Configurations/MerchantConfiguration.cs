using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OrderDelivery.Domain.Entities;

namespace OrderDelivery.Infrastructure.Configurations
{
    public class MerchantConfiguration : IEntityTypeConfiguration<Merchant>
    {
        public void Configure(EntityTypeBuilder<Merchant> builder)
        {
            // Table name
            builder.ToTable("Merchants");

            // Primary Key
            builder.HasKey(x => x.Id);

            // Foreign Key
            builder.Property(x => x.ApplicationUserId)
                .IsRequired();

            // Properties configuration
            builder.Property(x => x.StoreName)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(x => x.StoreAddress)
                .IsRequired()
                .HasMaxLength(500);

            builder.Property(x => x.StoreType)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(x => x.StoreLatitude)
                .IsRequired()
                .HasPrecision(18, 6);

            builder.Property(x => x.StoreLongitude)
                .IsRequired()
                .HasPrecision(18, 6);

            builder.Property(x => x.Rating)
                .HasPrecision(3, 2)
                .HasDefaultValue(0);

            builder.Property(x => x.TotalOrders)
                .HasDefaultValue(0);

            builder.Property(x => x.BusinessLicenseNumber)
                .HasMaxLength(100);

            // Relationships
            builder.HasOne(x => x.AppUser)
                .WithOne(x => x.Merchant)
                .HasForeignKey<Merchant>(x => x.ApplicationUserId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(x => x.Orders)
                .WithOne(x => x.Merchant)
                .HasForeignKey(x => x.MerchantId)
                .OnDelete(DeleteBehavior.Restrict);

            // Indexes
            builder.HasIndex(x => x.ApplicationUserId)
                .IsUnique();

            builder.HasIndex(x => new { x.StoreLatitude, x.StoreLongitude });
        }
    }
} 