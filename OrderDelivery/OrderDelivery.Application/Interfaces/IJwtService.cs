using OrderDelivery.Application.DTOs.Requests;
using OrderDelivery.Application.DTOs.Responses;
using System.Security.Claims;

namespace OrderDelivery.Application.Interfaces
{
    /// <summary>
    /// Interface for JWT token operations
    /// </summary>
    public interface IJwtService
    {
        /// <summary>
        /// Generates JWT access token for the specified user
        /// </summary>
        /// <param name="user">User information</param>
        /// <param name="roles">User roles</param>
        /// <returns>JWT access token</returns>
        string GenerateAccessToken(UserDto user, IList<string> roles);

        /// <summary>
        /// Generates refresh token
        /// </summary>
        /// <returns>Refresh token string</returns>
        string GenerateRefreshToken();

        /// <summary>
        /// Validates JWT token and extracts claims
        /// </summary>
        /// <param name="token">JWT token to validate</param>
        /// <returns>Claims principal if valid, null otherwise</returns>
        ClaimsPrincipal? ValidateToken(string token);

        /// <summary>
        /// Gets user ID from JWT token
        /// </summary>
        /// <param name="token">JWT token</param>
        /// <returns>User ID if valid, null otherwise</returns>
        string? GetUserIdFromToken(string token);

        /// <summary>
        /// Gets user roles from JWT token
        /// </summary>
        /// <param name="token">JWT token</param>
        /// <returns>List of user roles</returns>
        IList<string> GetUserRolesFromToken(string token);
    }
} 