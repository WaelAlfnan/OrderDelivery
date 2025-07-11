using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OrderDelivery.Domain.Entities;

namespace OrderDelivery.Infrastructure.Configurations
{
    public class OrderConfiguration : IEntityTypeConfiguration<Order>
    {
        public void Configure(EntityTypeBuilder<Order> builder)
        {
            // Table name
            builder.ToTable("Orders");

            // Primary Key
            builder.HasKey(x => x.Id);

            // Foreign Keys
            builder.Property(x => x.MerchantId)
                .IsRequired();

            builder.Property(x => x.DriverId)
                .IsRequired(false);

            // Properties configuration
            builder.Property(x => x.CustomerName)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(x => x.CustomerPhoneNumber)
                .IsRequired()
                .HasMaxLength(20);

            builder.Property(x => x.DeliveryAddress)
                .HasMaxLength(500);

            builder.Property(x => x.DeliveryLatitude)
                .IsRequired()
                .HasPrecision(18, 6);

            builder.Property(x => x.DeliveryLongitude)
                .IsRequired()
                .HasPrecision(18, 6);

            builder.Property(x => x.OrderValue)
                .IsRequired()
                .HasPrecision(18, 2);

            builder.Property(x => x.DeliveryFee)
                .IsRequired()
                .HasPrecision(18, 2);

            builder.Property(x => x.PaymentMethod)
                .IsRequired();

            builder.Property(x => x.Status)
                .IsRequired();

            builder.Property(x => x.SpecialInstructions)
                .HasMaxLength(1000);

            builder.Property(x => x.PlacedAt)
                .IsRequired()
                .HasDefaultValueSql("GETUTCDATE()");

            builder.Property(x => x.AssignedAt)
                .IsRequired(false);

            builder.Property(x => x.PickedUpAt)
                .IsRequired(false);

            builder.Property(x => x.DeliveredAt)
                .IsRequired(false);

            builder.Property(x => x.CancelledAt)
                .IsRequired(false);

            // Relationships
            builder.HasOne(x => x.Merchant)
                .WithMany(x => x.Orders)
                .HasForeignKey(x => x.MerchantId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.Driver)
                .WithMany(x => x.OrdersDelivered)
                .HasForeignKey(x => x.DriverId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(x => x.Ratings)
                .WithOne(x => x.Order)
                .HasForeignKey(x => x.OrderId)
                .OnDelete(DeleteBehavior.Cascade);

            // Indexes
            builder.HasIndex(x => x.MerchantId);
            builder.HasIndex(x => x.DriverId);
            builder.HasIndex(x => x.Status);
            builder.HasIndex(x => x.PlacedAt);
        }
    }
} 