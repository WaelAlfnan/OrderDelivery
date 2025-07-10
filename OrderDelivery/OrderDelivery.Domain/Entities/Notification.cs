using OrderDelivery.Domain.Enums;

namespace OrderDelivery.Domain.Entities
{
    public class Notification
    {
        public Guid Id { get; set; }

        public Guid ApplicationUserId { get; set; }
        public MessageType MessageType { get; set; }
        public string MessageContent { get; set; } = string.Empty;
        public bool IsRead { get; set; } = false;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Navigation Property
        public virtual ApplicationUser AppUser { get; set; } = null!;
    }
}