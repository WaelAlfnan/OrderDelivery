using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OrderDelivery.Domain.Entities;

namespace OrderDelivery.Infrastructure.Configurations
{
    public class RefreshTokenConfiguration : IEntityTypeConfiguration<RefreshToken>
    {
        public void Configure(EntityTypeBuilder<RefreshToken> builder)
        {
            // Table name
            builder.ToTable("RefreshTokens");

            // Primary Key
            builder.HasKey(x => x.Id);

            // Properties configuration
            builder.Property(x => x.Token)
                .IsRequired()
                .HasMaxLength(1000);

            builder.Property(x => x.ExpiresOn)
                .IsRequired();

            builder.Property(x => x.CreatedOn)
                .IsRequired();

            builder.Property(x => x.RevokedReason)
                .HasMaxLength(200);

            builder.Property(x => x.CreatedByIp)
                .HasMaxLength(45);

            builder.Property(x => x.RevokedByIp)
                .HasMaxLength(45);

            builder.Property(x => x.ReplacedByToken)
                .HasMaxLength(1000);

            // Foreign Key relationship with ApplicationUser
            builder.HasOne(x => x.ApplicationUser)
                .WithMany(x => x.RefreshTokens)
                .HasForeignKey(x => x.ApplicationUserId)
                .OnDelete(DeleteBehavior.Cascade);

            // Index on Token for faster lookups
            builder.HasIndex(x => x.Token)
                .IsUnique();

            // Index on ApplicationUserId for faster queries
            builder.HasIndex(x => x.ApplicationUserId);
        }
    }
}
