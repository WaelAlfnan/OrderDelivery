using Microsoft.AspNetCore.Identity;
using OrderDelivery.Domain.Enums;

namespace OrderDelivery.Domain.Entities
{
    public class ApplicationUser : IdentityUser<Guid>
    {
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string PersonalPhotoUrl { get; set; } = string.Empty;
        public string NationalIdFrontPhotoUrl { get; set; } = string.Empty;
        public string NationalIdBackPhotoUrl { get; set; } = string.Empty;
        public string NationalIdNumber { get; set; } = string.Empty;


        // Navigation Properties (One-to-One)
        public virtual Merchant? Merchant { get; set; }
        public virtual Driver? Driver { get; set; }

        // Navigation Properties (One-to-Many)
        public virtual ICollection<Rating> RatingsGiven { get; set; } = new List<Rating>();
        public virtual ICollection<Rating> RatingsReceived { get; set; } = new List<Rating>();
        public ICollection<Notification> Notifications { get; set; } = new List<Notification>();


    }
}