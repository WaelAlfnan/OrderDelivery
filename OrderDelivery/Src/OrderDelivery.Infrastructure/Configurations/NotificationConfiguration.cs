using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OrderDelivery.Domain.Entities;

namespace OrderDelivery.Infrastructure.Configurations
{
    public class NotificationConfiguration : IEntityTypeConfiguration<Notification>
    {
        public void Configure(EntityTypeBuilder<Notification> builder)
        {
            // Table name
            builder.ToTable("Notifications");

            // Primary Key
            builder.HasKey(x => x.Id);

            // Foreign Key
            builder.Property(x => x.ApplicationUserId)
                .IsRequired();

            // Properties configuration
            builder.Property(x => x.MessageType)
                .IsRequired();

            builder.Property(x => x.MessageContent)
                .IsRequired()
                .HasMaxLength(1000);

            builder.Property(x => x.IsRead)
                .HasDefaultValue(false);

            builder.Property(x => x.CreatedAt)
                .HasDefaultValueSql("GETUTCDATE()");

            // Relationships
            builder.HasOne(x => x.AppUser)
                .WithMany(x => x.Notifications)
                .HasForeignKey(x => x.ApplicationUserId)
                .OnDelete(DeleteBehavior.Cascade);

            // Indexes
            builder.HasIndex(x => x.ApplicationUserId);
            builder.HasIndex(x => x.MessageType);
        }
    }
} 