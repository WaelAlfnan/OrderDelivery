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
        /// Logs out user and revokes all refresh tokens
        /// </summary>
        /// <param name="userId">User ID to logout</param>
        /// <returns>Logout result</returns>
        Task<(bool Success, string Message)> LogoutAsync(Guid userId);

        /// <summary>
        /// Initiates password reset process
        /// </summary>
        /// <param name="forgotPasswordDto">Forgot password data</param>
        /// <returns>Success result with session token</returns>
        Task<(bool Success, string Message, string? SessionToken)> ForgotPasswordAsync(ForgotPasswordDto forgotPasswordDto);

        /// <summary>
        /// Resends verification code for password reset
        /// </summary>
        /// <param name="resendCodeDto">Resend code data</param>
        /// <returns>Success result with new session token</returns>
        Task<(bool Success, string Message, string? SessionToken)> ResendVerificationCodeAsync(ResendCodeDto resendCodeDto);

        /// <summary>
        /// Verifies the verification code for password reset
        /// </summary>
        /// <param name="verifyCodeDto">Verification code data</param>
        /// <returns>Success result with verification code and new session token</returns>
        Task<(bool Success, string Message, string? VerificationCode, string? SessionToken)> VerifyCodeAsync(VerifyCodeDto verifyCodeDto);

        /// <summary>
        /// Sets new password after verification code validation
        /// </summary>
        /// <param name="setNewPasswordDto">New password data</param>
        /// <returns>Success result</returns>
        Task<(bool Success, string Message)> SetNewPasswordAsync(SetNewPasswordDto setNewPasswordDto);

        /// <summary>
        /// Validates password reset session
        /// </summary>
        /// <param name="sessionDto">Session data</param>
        /// <returns>Session validation result</returns>
        Task<(bool Success, string Message)> ValidatePasswordResetSessionAsync(PasswordResetSessionDto sessionDto);

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