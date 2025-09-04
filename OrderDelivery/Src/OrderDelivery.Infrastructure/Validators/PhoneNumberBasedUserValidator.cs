using Microsoft.AspNetCore.Identity;
using OrderDelivery.Domain.Constants;
using OrderDelivery.Domain.Entities;

namespace OrderDelivery.Infrastructure.Validators
{
    /// <summary>
    /// Custom user validator that allows phone number-based authentication without requiring email
    /// </summary>
    public class PhoneNumberBasedUserValidator<TUser> : UserValidator<TUser> where TUser : ApplicationUser
    {
        public PhoneNumberBasedUserValidator()
        {
        }

        public override async Task<IdentityResult> ValidateAsync(UserManager<TUser> manager, TUser user)
        {
            var result = await base.ValidateAsync(manager, user);

            // Remove email-related errors since we don't require email
            if (!result.Succeeded)
            {
                var errors = result.Errors.Where(e =>
                    !e.Code.Contains("Email") &&
                    !e.Code.Contains("DuplicateEmail") &&
                    !e.Code.Contains("InvalidEmail")).ToList();

                if (errors.Any())
                {
                    return IdentityResult.Failed(errors.ToArray());
                }
            }

            // Validate that phone number is provided and unique
            if (string.IsNullOrEmpty(user.PhoneNumber))
            {
                return IdentityResult.Failed(new IdentityError
                {
                    Code = "PhoneNumberRequired",
                    Description = "Phone number is required."
                });
            }

            // Validate phone number format (basic validation)
            if (!System.Text.RegularExpressions.Regex.IsMatch(user.PhoneNumber, ValidationPatterns.PhoneNumber))
            {
                return IdentityResult.Failed(new IdentityError
                {
                    Code = "InvalidPhoneNumber",
                    Description = "Phone number format is invalid. Must be 10-15 digits with optional + prefix."
                });
            }

            // Ensure UserName and PhoneNumber are synchronized
            if (string.IsNullOrEmpty(user.UserName) || user.UserName != user.PhoneNumber)
            {
                user.UserName = user.PhoneNumber;
            }

            // Check if phone number is already used by another user
            var existingUser = await manager.FindByNameAsync(user.PhoneNumber);
            if (existingUser != null && existingUser.Id != user.Id)
            {
                return IdentityResult.Failed(new IdentityError
                {
                    Code = "DuplicatePhoneNumber",
                    Description = "Phone number is already in use."
                });
            }

            // Ensure email is not null (set to empty string if needed)
            if (user.Email == null)
            {
                user.Email = string.Empty;
            }

            return IdentityResult.Success;
        }
    }
}
