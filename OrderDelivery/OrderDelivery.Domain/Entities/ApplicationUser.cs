using OrderDelivery.Domain.Enums;

namespace OrderDelivery.Domain.Entities
{
    public class ApplicationUser
    {
        public Guid Id { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string PersonalPhotoUrl { get; set; } = string.Empty;
        public string NationalIdFrontPhotoUrl { get; set; } = string.Empty;
        public string NationalIdBackPhotoUrl { get; set; } = string.Empty;
        public string NationalIdNumber { get; set; } = string.Empty;
        public UserType UserType { get; set; }
        public string PhoneNumber { get; set; } = string.Empty;
        public bool LockoutEnabled { get; set; }


        // Navigation Properties (One-to-One)
        public virtual Merchant? Merchant { get; set; }
        public virtual Driver? Driver { get; set; }
        public virtual Admin? Admin { get; set; }

        // Navigation Properties (One-to-Many)
        public virtual ICollection<Rating> RatingsGiven { get; set; } = new List<Rating>();
        public virtual ICollection<Rating> RatingsReceived { get; set; } = new List<Rating>();
        public ICollection<Notification> Notifications { get; set; } = new List<Notification>();


    }
}