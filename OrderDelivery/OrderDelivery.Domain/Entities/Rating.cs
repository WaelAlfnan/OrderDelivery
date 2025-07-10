namespace OrderDelivery.Domain.Entities
{
    public class Rating
    {
        public Guid RatingId { get; set; } // Primary Key

        public Guid OrderId { get; set; } // Foreign Key to Order (الطلب الذي تم تقييمه)
        public Guid EvaluatorApplicationUserId { get; set; }
        public Guid RatedApplicationUserId { get; set; }

        public int Stars { get; set; } // عدد النجوم (مثلاً من 1 إلى 5)
        public string? Comment { get; set; } // تعليق على التقييم
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow; // تاريخ ووقت التقييم

        // Navigation Properties
        public virtual Order Order { get; set; } = null!; // Required navigation property
        public virtual ApplicationUser EvaluatorUser { get; set; } = null!; // المستخدم الذي قام بالتقييم
        public virtual ApplicationUser RatedUser { get; set; } = null!; // المستخدم الذي تم تقييمه
    }
}