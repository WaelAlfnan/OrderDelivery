using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OrderDelivery.Domain.Entities;

namespace OrderDelivery.Infrastructure.Configurations
{
    public class ApplicationUserConfiguration : IEntityTypeConfiguration<ApplicationUser>
    {
        public void Configure(EntityTypeBuilder<ApplicationUser> builder)
        {
            // Table name
            builder.ToTable("Users");

            // Primary Key
            builder.HasKey(x => x.Id);

            // Properties configuration
            builder.Property(x => x.FirstName)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(x => x.LastName)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(x => x.PersonalPhotoUrl)
                .IsRequired()
                .HasMaxLength(500);

            builder.Property(x => x.NationalIdFrontPhotoUrl)
                .IsRequired()
                .HasMaxLength(500);

            builder.Property(x => x.NationalIdBackPhotoUrl)
                .IsRequired()
                .HasMaxLength(500);

            builder.Property(x => x.NationalIdNumber)
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(x => x.PhoneNumber)
                .IsRequired()
                .HasMaxLength(20);

            // Ensure phone number uniqueness at the database level
            builder.HasIndex(x => x.PhoneNumber)
                .IsUnique();

            builder.Property(x => x.LockoutEnabled)
                .IsRequired()
                .HasDefaultValue(false);

            // One-to-One relationships
            builder.HasOne(x => x.Merchant)
                .WithOne(x => x.AppUser)
                .HasForeignKey<Merchant>(x => x.ApplicationUserId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(x => x.Driver)
                .WithOne(x => x.AppUser)
                .HasForeignKey<Driver>(x => x.ApplicationUserId)
                .OnDelete(DeleteBehavior.Cascade);

            // One-to-Many relationships
            builder.HasMany(x => x.RatingsGiven)
                .WithOne(x => x.EvaluatorUser)
                .HasForeignKey(x => x.EvaluatorApplicationUserId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(x => x.RatingsReceived)
                .WithOne(x => x.RatedUser)
                .HasForeignKey(x => x.RatedApplicationUserId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(x => x.Notifications)
                .WithOne(x => x.AppUser)
                .HasForeignKey(x => x.ApplicationUserId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
} 