using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OrderDelivery.Domain.Entities;

namespace OrderDelivery.Infrastructure.Configurations
{
    public class AdminConfiguration : IEntityTypeConfiguration<Admin>
    {
        public void Configure(EntityTypeBuilder<Admin> builder)
        {
            // Table name
            builder.ToTable("Admins");

            // Primary Key
            builder.HasKey(x => x.Id);

            // Foreign Key
            builder.Property(x => x.ApplicationUserId)
                .IsRequired();

            // Properties configuration
            builder.Property(x => x.Role)
                .IsRequired();

            builder.Property(x => x.CanManageMerchants)
                .IsRequired()
                .HasDefaultValue(false);

            builder.Property(x => x.CanManageDrivers)
                .IsRequired()
                .HasDefaultValue(false);

            builder.Property(x => x.CanManageOrders)
                .IsRequired()
                .HasDefaultValue(false);

            builder.Property(x => x.CanViewReports)
                .IsRequired()
                .HasDefaultValue(false);

            // Relationships
            builder.HasOne(x => x.AppUser)
                .WithOne(x => x.Admin)
                .HasForeignKey<Admin>(x => x.ApplicationUserId)
                .OnDelete(DeleteBehavior.Cascade);

            // Indexes
            builder.HasIndex(x => x.ApplicationUserId)
                .IsUnique();
        }
    }
} 