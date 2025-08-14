using OrderDelivery.Domain.Entities;

namespace OrderDelivery.Application.Interfaces;

public interface IUserCreationService
{
    Task<(bool Success, string Message, ApplicationUser? User)> CreateUserAsync(
        string userName, 
        string phoneNumber, 
        string password, 
        string firstName, 
        string lastName,
        string personalPhotoUrl,
        string nationalIdFrontPhotoUrl,
        string nationalIdBackPhotoUrl,
        string nationalIdNumber);
    
    Task<(bool Success, string Message)> AssignRoleAsync(ApplicationUser user, string role);
    
    Task<bool> RoleExistsAsync(string role);
    
    Task<bool> CreateRoleAsync(string role);
} 