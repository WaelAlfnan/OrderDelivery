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


        // Navigation Properties
        public virtual Merchant? Merchant { get; set; }
        public virtual Driver? Driver { get; set; }

    }
}