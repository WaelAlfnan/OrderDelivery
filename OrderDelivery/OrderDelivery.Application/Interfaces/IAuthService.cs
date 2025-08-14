using OrderDelivery.Application.DTOs.Requests;
using OrderDelivery.Application.DTOs.Responses;

namespace OrderDelivery.Application.Interfaces
{
    /// <summary>
    /// Interface for authentication service operations
    /// </summary>
    public interface IAuthService
    {
        /// <summary>
        /// Authenticates user and generates JWT token
        /// </summary>
        /// <param name="loginDto">User login data</param>
        /// <returns>Authentication response with JWT token</returns>
        Task<(bool Success, string Message, AuthResponseDto? AuthResponse)> LoginAsync(LoginDto loginDto);

        /// <summary>
        /// Initiates password reset process
        /// </summary>
        /// <param name="forgotPasswordDto">Forgot password data</param>
        /// <returns>Success result</returns>
        Task<(bool Success, string Message)> ForgotPasswordAsync(ForgotPasswordDto forgotPasswordDto);

        /// <summary>
        /// Resets password with verification code
        /// </summary>
        /// <param name="resetPasswordDto">Password reset data</param>
        /// <returns>Success result</returns>
        Task<(bool Success, string Message)> ResetPasswordAsync(ResetPasswordDto resetPasswordDto);

        /// <summary>
        /// Gets user profile by ID
        /// </summary>
        /// <param name="userId">User ID</param>
        /// <returns>User information</returns>
        Task<(bool Success, string Message, UserDto? User)> GetUserProfileAsync(string userId);

        /// <summary>
        /// Refreshes JWT token using refresh token
        /// </summary>
        /// <param name="refreshToken">Refresh token</param>
        /// <returns>New authentication response</returns>
        Task<(bool Success, string Message, AuthResponseDto? AuthResponse)> RefreshTokenAsync(string refreshToken);
    }
} 