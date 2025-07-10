using Microsoft.AspNetCore.Identity;
using OrderDelivery.Domain.Enums;

namespace OrderDelivery.Infrastructure.Data
{
    public class User : IdentityUser<Guid>
    {
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string PersonalPhotoUrl { get; set; } = string.Empty;
        public string NationalIdFrontPhotoUrl { get; set; } = string.Empty;
        public string NationalIdBackPhotoUrl { get; set; } = string.Empty;
        public string NationalIdNumber { get; set; } = string.Empty;
        public UserType UserType { get; set; }
    }
}
