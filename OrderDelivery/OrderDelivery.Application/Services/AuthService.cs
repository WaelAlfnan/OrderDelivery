using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using OrderDelivery.Application.DTOs.Requests;
using OrderDelivery.Application.DTOs.Responses;
using OrderDelivery.Application.Interfaces;
using OrderDelivery.Application.Mappers.RegistrationStepsMappers;
using OrderDelivery.Domain.Entities;
using System.Security.Cryptography;

namespace OrderDelivery.Application.Services
{
    /// <summary>
    /// Main authentication service implementation
    /// </summary>
    public class AuthService : IAuthService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IJwtService _jwtService;
        private readonly ISmsService _smsService;
        private readonly ILogger<AuthService> _logger;

        // In-memory storage for verification codes (in production, use Redis or database)
        private static readonly Dictionary<string, (string Code, DateTime Expiry)> _verificationCodes = new();

        public AuthService(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            IJwtService jwtService,
            ISmsService smsService,
            ILogger<AuthService> logger)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _jwtService = jwtService;
            _smsService = smsService;
            _logger = logger;
        }

        /// <summary>
        /// Authenticates user and generates JWT token
        /// </summary>
        public async Task<(bool Success, string Message, AuthResponseDto? AuthResponse)> LoginAsync(LoginDto loginDto)
        {
            try
            {
                // Find user by phone number
                var user = await _userManager.FindByNameAsync(loginDto.PhoneNumber);
                if (user == null)
                {
                    return (false, "Invalid phone number or password.", null);
                }

                // Check password
                var passwordValid = await _userManager.CheckPasswordAsync(user, loginDto.Password);
                if (!passwordValid)
                {
                    return (false, "Invalid phone number or password.", null);
                }

                // Get user roles
                var roles = await _userManager.GetRolesAsync(user);

                // Create user DTO using mapper
                var userDto = user.ToUserDto(roles.FirstOrDefault() ?? string.Empty);

                // Generate JWT token
                var accessToken = _jwtService.GenerateAccessToken(userDto, roles.ToList());
                var refreshToken = _jwtService.GenerateRefreshToken();

                var authResponse = new AuthResponseDto(
                    AccessToken: accessToken,
                    RefreshToken: refreshToken,
                    ExpiresAt: DateTime.UtcNow.AddMinutes(60), // Should come from JWT settings
                    User: userDto
                );

                return (true, "Login successful.", authResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during login for {PhoneNumber}", loginDto.PhoneNumber);
                return (false, "An error occurred during login.", null);
            }
        }

        /// <summary>
        /// Initiates password reset process
        /// </summary>
        public async Task<(bool Success, string Message)> ForgotPasswordAsync(ForgotPasswordDto forgotPasswordDto)
        {
            try
            {
                // Find user
                var user = await _userManager.FindByNameAsync(forgotPasswordDto.PhoneNumber);
                if (user == null)
                {
                    // Don't reveal if user exists or not for security
                    return (true, "If the phone number exists, a reset code has been sent.");
                }

                // Generate reset code
                var resetCode = GenerateVerificationCode();
                _verificationCodes[$"{forgotPasswordDto.PhoneNumber}_reset"] = (resetCode, DateTime.UtcNow.AddMinutes(10));

                // Send SMS
                var smsSent = await _smsService.SendPasswordResetCodeAsync(forgotPasswordDto.PhoneNumber, resetCode);
                if (!smsSent)
                {
                    _logger.LogWarning("Failed to send password reset SMS to {PhoneNumber}", forgotPasswordDto.PhoneNumber);
                }

                return (true, "If the phone number exists, a reset code has been sent.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during forgot password for {PhoneNumber}", forgotPasswordDto.PhoneNumber);
                return (false, "An error occurred during password reset process.");
            }
        }

        /// <summary>
        /// Resets password with verification code
        /// </summary>
        public async Task<(bool Success, string Message)> ResetPasswordAsync(ResetPasswordDto resetPasswordDto)
        {
            try
            {
                // Find user
                var user = await _userManager.FindByNameAsync(resetPasswordDto.PhoneNumber);
                if (user == null)
                {
                    return (false, "User not found.");
                }

                // Check reset code
                var codeKey = $"{resetPasswordDto.PhoneNumber}_reset";
                if (!_verificationCodes.TryGetValue(codeKey, out var codeInfo))
                {
                    return (false, "Reset code not found or expired.");
                }

                if (DateTime.UtcNow > codeInfo.Expiry)
                {
                    _verificationCodes.Remove(codeKey);
                    return (false, "Reset code has expired.");
                }

                if (codeInfo.Code != resetPasswordDto.VerificationCode)
                {
                    return (false, "Invalid reset code.");
                }

                // Reset password
                var token = await _userManager.GeneratePasswordResetTokenAsync(user);
                var result = await _userManager.ResetPasswordAsync(user, token, resetPasswordDto.NewPassword);
                if (!result.Succeeded)
                {
                    var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                    return (false, $"Failed to reset password: {errors}");
                }

                // Remove reset code
                _verificationCodes.Remove(codeKey);

                return (true, "Password reset successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during password reset for {PhoneNumber}", resetPasswordDto.PhoneNumber);
                return (false, "An error occurred during password reset.");
            }
        }

        /// <summary>
        /// Gets user profile by ID
        /// </summary>
        public async Task<(bool Success, string Message, UserDto? User)> GetUserProfileAsync(string userId)
        {
            try
            {
                if (!Guid.TryParse(userId, out var userGuid))
                {
                    return (false, "Invalid user ID.", null);
                }

                var user = await _userManager.FindByIdAsync(userId);
                if (user == null)
                {
                    return (false, "User not found.", null);
                }

                var roles = await _userManager.GetRolesAsync(user);

                var userDto = user.ToUserDto(roles.FirstOrDefault() ?? string.Empty);

                return (true, "User profile retrieved successfully.", userDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving user profile for {UserId}", userId);
                return (false, "An error occurred while retrieving user profile.", null);
            }
        }

        /// <summary>
        /// Refreshes JWT token using refresh token
        /// </summary>
        public async Task<(bool Success, string Message, AuthResponseDto? AuthResponse)> RefreshTokenAsync(string refreshToken)
        {
            // This is a simplified implementation
            // In production, you should store refresh tokens in database and validate them properly
            try
            {
                // For now, we'll just generate a new token
                // In a real implementation, you would validate the refresh token against stored tokens
                return (false, "Refresh token functionality not implemented yet.", null);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during token refresh");
                return (false, "An error occurred during token refresh.", null);
            }
        }

        /// <summary>
        /// Generates a 6-digit verification code
        /// </summary>
        private static string GenerateVerificationCode()
        {
            return RandomNumberGenerator.GetInt32(100000, 999999).ToString();
        }
    }
}