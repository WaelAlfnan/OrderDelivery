using OrderDelivery.Application.DTOs.Responses;

namespace OrderDelivery.Application.Interfaces
{
    /// <summary>
    /// Interface for refresh token management service
    /// </summary>
    public interface IRefreshTokenService
    {
        /// <summary>
        /// Creates a new refresh token for a user
        /// </summary>
        /// <param name="userId">User ID</param>
        /// <param name="ipAddress">IP address of the request</param>
        /// <returns>Refresh token string</returns>
        Task<string> CreateRefreshTokenAsync(Guid userId, string? ipAddress = null);

        /// <summary>
        /// Validates and refreshes a token
        /// </summary>
        /// <param name="refreshToken">Refresh token to validate</param>
        /// <returns>New authentication response</returns>
        Task<(bool Success, string Message, AuthResponseDto? AuthResponse)> RefreshTokenAsync(string refreshToken);

        /// <summary>
        /// Revokes all refresh tokens for a user
        /// </summary>
        /// <param name="userId">User ID</param>
        /// <param name="reason">Reason for revocation</param>
        /// <returns>Success result</returns>
        Task<bool> RevokeAllUserTokensAsync(Guid userId, string reason);

        /// <summary>
        /// Revokes a specific refresh token
        /// </summary>
        /// <param name="refreshToken">Refresh token to revoke</param>
        /// <param name="reason">Reason for revocation</param>
        /// <param name="ipAddress">IP address of the request</param>
        /// <returns>Success result</returns>
        Task<bool> RevokeTokenAsync(string refreshToken, string reason, string? ipAddress = null);

        /// <summary>
        /// Revokes a specific refresh token (alias for RevokeTokenAsync)
        /// </summary>
        /// <param name="refreshToken">Refresh token to revoke</param>
        /// <returns>Success result</returns>
        Task<bool> RevokeRefreshTokenAsync(string refreshToken);

        /// <summary>
        /// Revokes all refresh tokens for a user (alias for RevokeAllUserTokensAsync)
        /// </summary>
        /// <param name="userId">User ID</param>
        /// <returns>Success result</returns>
        Task<bool> RevokeAllRefreshTokensAsync(Guid userId);

        /// <summary>
        /// Validates if a refresh token is still valid
        /// </summary>
        /// <param name="refreshToken">Refresh token to validate</param>
        /// <returns>True if valid, false otherwise</returns>
        Task<bool> IsRefreshTokenValidAsync(string refreshToken);
    }
}
