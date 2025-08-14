using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using OrderDelivery.Application.Interfaces;
using OrderDelivery.Domain.Entities;

namespace OrderDelivery.Application.Services;

public class UserCreationService : IUserCreationService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly RoleManager<IdentityRole<Guid>> _roleManager;
    private readonly ILogger<UserCreationService> _logger;

    public UserCreationService(
        UserManager<ApplicationUser> userManager,
        RoleManager<IdentityRole<Guid>> roleManager,
        ILogger<UserCreationService> logger)
    {
        _userManager = userManager;
        _roleManager = roleManager;
        _logger = logger;
    }

    public async Task<(bool Success, string Message, ApplicationUser? User)> CreateUserAsync(
        string userName,
        string phoneNumber,
        string password,
        string firstName,
        string lastName,
        string personalPhotoUrl,
        string nationalIdFrontPhotoUrl,
        string nationalIdBackPhotoUrl,
        string nationalIdNumber)
    {
        try
        {
            var user = new ApplicationUser
            {
                UserName = userName,
                PhoneNumber = phoneNumber,
                FirstName = firstName,
                LastName = lastName,
                PersonalPhotoUrl = personalPhotoUrl,
                NationalIdFrontPhotoUrl = nationalIdFrontPhotoUrl,
                NationalIdBackPhotoUrl = nationalIdBackPhotoUrl,
                NationalIdNumber = nationalIdNumber,
                PhoneNumberConfirmed = true,
                EmailConfirmed = false,
                TwoFactorEnabled = false,
                LockoutEnabled = false
            };

            var result = await _userManager.CreateAsync(user, password);
            if (!result.Succeeded)
            {
                var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                _logger.LogError("Failed to create user: {Errors}", errors);
                return (false, $"Failed to create user: {errors}", null);
            }

            return (true, "User created successfully", user);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating user for {PhoneNumber}", phoneNumber);
            return (false, "An error occurred while creating user", null);
        }
    }

    public async Task<(bool Success, string Message)> AssignRoleAsync(ApplicationUser user, string role)
    {
        try
        {
            var result = await _userManager.AddToRoleAsync(user, role);
            if (!result.Succeeded)
            {
                var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                _logger.LogError("Failed to assign role: {Errors}", errors);
                return (false, $"Failed to assign role: {errors}");
            }

            return (true, "Role assigned successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error assigning role {Role} to user {UserId}", role, user.Id);
            return (false, "An error occurred while assigning role");
        }
    }

    public async Task<bool> RoleExistsAsync(string role)
    {
        return await _roleManager.RoleExistsAsync(role);
    }

    public async Task<bool> CreateRoleAsync(string role)
    {
        try
        {
            var result = await _roleManager.CreateAsync(new IdentityRole<Guid>(role));
            return result.Succeeded;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating role {Role}", role);
            return false;
        }
    }
}