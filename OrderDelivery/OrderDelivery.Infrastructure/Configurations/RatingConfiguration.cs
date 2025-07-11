using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OrderDelivery.Domain.Entities;

namespace OrderDelivery.Infrastructure.Configurations
{
    public class RatingConfiguration : IEntityTypeConfiguration<Rating>
    {
        public void Configure(EntityTypeBuilder<Rating> builder)
        {
            // Table name
            builder.ToTable("Ratings");

            // Primary Key
            builder.HasKey(x => x.Id);

            // Foreign Keys
            builder.Property(x => x.OrderId)
                .IsRequired();

            builder.Property(x => x.EvaluatorApplicationUserId)
                .IsRequired();

            builder.Property(x => x.RatedApplicationUserId)
                .IsRequired();

            // Properties configuration
            builder.Property(x => x.Stars)
                .IsRequired()
                .HasDefaultValue(0);

            builder.Property(x => x.Comment)
                .HasMaxLength(1000);

            builder.Property(x => x.CreatedAt)
                .IsRequired()
                .HasDefaultValueSql("GETUTCDATE()");

            // Relationships
            builder.HasOne(x => x.Order)
                .WithMany(x => x.Ratings)
                .HasForeignKey(x => x.OrderId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(x => x.EvaluatorUser)
                .WithMany(x => x.RatingsGiven)
                .HasForeignKey(x => x.EvaluatorApplicationUserId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.RatedUser)
                .WithMany(x => x.RatingsReceived)
                .HasForeignKey(x => x.RatedApplicationUserId)
                .OnDelete(DeleteBehavior.Restrict);

            // Indexes
            builder.HasIndex(x => x.OrderId);
            builder.HasIndex(x => x.EvaluatorApplicationUserId);
            builder.HasIndex(x => x.RatedApplicationUserId);
            builder.HasIndex(x => x.CreatedAt);

            // Constraints
            builder.HasCheckConstraint("CK_Rating_Stars", "[Stars] >= 1 AND [Stars] <= 5");
        }
    }
} 