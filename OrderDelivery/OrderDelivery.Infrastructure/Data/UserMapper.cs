using OrderDelivery.Domain.Entities;

namespace OrderDelivery.Infrastructure.Data
{
    public static class UserMapper
    {
        public static User ToUser(ApplicationUser appUser)
        {
            if (appUser == null) return null;
            return new User
            {
                FirstName = appUser.FirstName,
                LastName = appUser.LastName,
                PersonalPhotoUrl = appUser.PersonalPhotoUrl,
                NationalIdFrontPhotoUrl = appUser.NationalIdFrontPhotoUrl,
                NationalIdBackPhotoUrl = appUser.NationalIdBackPhotoUrl,
                NationalIdNumber = appUser.NationalIdNumber,
                UserType = appUser.UserType,
                PhoneNumber = appUser.PhoneNumber,
                LockoutEnabled = appUser.LockoutEnabled
            };
        }

        public static ApplicationUser ToApplicationUser(User user)
        {
            if (user == null) return null;
            return new ApplicationUser
            {
                Id = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                PersonalPhotoUrl = user.PersonalPhotoUrl,
                NationalIdFrontPhotoUrl = user.NationalIdFrontPhotoUrl,
                NationalIdBackPhotoUrl = user.NationalIdBackPhotoUrl,
                NationalIdNumber = user.NationalIdNumber,
                UserType = user.UserType,
                PhoneNumber = user.PhoneNumber,
                LockoutEnabled = user.LockoutEnabled
            };
        }
    }
}
