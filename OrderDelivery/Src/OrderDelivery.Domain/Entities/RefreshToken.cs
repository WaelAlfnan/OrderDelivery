using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace OrderDelivery.Domain.Entities
{
    public class RefreshToken
    {
        public Guid Id { get; set; }
        
        [Required]
        [MaxLength(1000)]
        public string Token { get; set; } = string.Empty;

        [Required]
        public DateTime ExpiresOn { get; set; }

        [Required]
        public DateTime CreatedOn { get; set; }

        public DateTime? RevokedOn { get; set; }

        [MaxLength(200)]
        public string? RevokedReason { get; set; }

        [MaxLength(45)]
        public string? CreatedByIp { get; set; }

        [MaxLength(45)]
        public string? RevokedByIp { get; set; }

        [MaxLength(1000)]
        public string? ReplacedByToken { get; set; }

        // Foreign Key to ApplicationUser
        public Guid ApplicationUserId { get; set; }
        public virtual ApplicationUser ApplicationUser { get; set; } = null!;

        // Computed Properties
        public bool IsExpired => DateTime.UtcNow >= ExpiresOn;
        public bool IsActive => RevokedOn == null && !IsExpired;
        public bool IsRevoked => RevokedOn != null;
        public TimeSpan TimeUntilExpiry => ExpiresOn - DateTime.UtcNow;
    }
}

