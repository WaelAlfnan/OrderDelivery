using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using OrderDelivery.Application.DTOs.Requests;
using OrderDelivery.Application.DTOs.Responses;
using OrderDelivery.Application.Interfaces;
using OrderDelivery.Application.Mappers.RegistrationStepsMappers;
using OrderDelivery.Domain.Entities;

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
        private readonly IRefreshTokenService _refreshTokenService;
        private readonly ILogger<AuthService> _logger;

        // In-memory storage for verification codes (in production, use Redis or database)
        private static readonly Dictionary<string, (string Code, DateTime Expiry)> _verificationCodes = new();

        // In-memory storage for password reset sessions (in production, use Redis or database)
        private static readonly Dictionary<string, (string PhoneNumber, DateTime Expiry, string Status)> _passwordResetSessions = new();

        public AuthService(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            IJwtService jwtService,
            ISmsService smsService,
            IRefreshTokenService refreshTokenService,
            ILogger<AuthService> logger)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _jwtService = jwtService;
            _smsService = smsService;
            _refreshTokenService = refreshTokenService;
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
                var accessToken = _jwtService.GenerateAccessToken(user.Id.ToString(), userDto, roles.ToList());

                // Create refresh token using the service
                var refreshToken = await _refreshTokenService.CreateRefreshTokenAsync(user.Id, null); // IP will be captured in controller

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
        /// Logs out user and revokes all refresh tokens
        /// </summary>
        public async Task<(bool Success, string Message)> LogoutAsync(Guid userId)
        {
            try
            {
                await _refreshTokenService.RevokeAllUserTokensAsync(userId, "User logout");
                return (true, "Logout successful.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during logout for user {UserId}", userId);
                return (false, "An error occurred during logout.");
            }
        }

        /// <summary>
        /// Initiates password reset process
        /// </summary>
        public async Task<(bool Success, string Message, string? SessionToken)> ForgotPasswordAsync(ForgotPasswordDto forgotPasswordDto)
        {
            try
            {
                // Find user
                var user = await _userManager.FindByNameAsync(forgotPasswordDto.PhoneNumber);
                if (user == null)
                {
                    // Don't reveal if user exists or not for security
                    return (true, "If the phone number exists, a reset code has been sent.", null);
                }

                // Generate reset code
                var resetCode = GenerateVerificationCode();
                _verificationCodes[$"{forgotPasswordDto.PhoneNumber}_reset"] = (resetCode, DateTime.UtcNow.AddMinutes(10));

                // Generate session token
                var sessionToken = GenerateSessionToken();
                _passwordResetSessions[sessionToken] = (forgotPasswordDto.PhoneNumber, DateTime.UtcNow.AddMinutes(10), "code_sent");

                // Send SMS
                var smsSent = await _smsService.SendPasswordResetCodeAsync(forgotPasswordDto.PhoneNumber, resetCode);
                if (!smsSent)
                {
                    _logger.LogWarning("Failed to send password reset SMS to {PhoneNumber}", forgotPasswordDto.PhoneNumber);
                }

                return (true, "If the phone number exists, a reset code has been sent.", sessionToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during forgot password for {PhoneNumber}", forgotPasswordDto.PhoneNumber);
                return (false, "An error occurred during password reset process.", null);
            }
        }

        /// <summary>
        /// Resends verification code for password reset
        /// </summary>
        public async Task<(bool Success, string Message, string? SessionToken)> ResendVerificationCodeAsync(ResendCodeDto resendCodeDto)
        {
            try
            {
                // Validate session first
                var sessionValidation = await ValidatePasswordResetSessionAsync(new PasswordResetSessionDto(resendCodeDto.PhoneNumber, resendCodeDto.SessionToken));
                if (!sessionValidation.Success)
                {
                    return (false, sessionValidation.Message, null);
                }

                // Remove old session and generate new one
                _passwordResetSessions.Remove(resendCodeDto.SessionToken);
                var newSessionToken = GenerateSessionToken();
                _passwordResetSessions[newSessionToken] = (resendCodeDto.PhoneNumber, DateTime.UtcNow.AddMinutes(10), "code_resent");

                // Find user
                var user = await _userManager.FindByNameAsync(resendCodeDto.PhoneNumber);
                if (user == null)
                {
                    // Don't reveal if user exists or not for security
                    return (true, "If the phone number exists, a new reset code has been sent.", newSessionToken);
                }

                // Check if there's an existing code and if it's expired
                var codeKey = $"{resendCodeDto.PhoneNumber}_reset";
                if (_verificationCodes.TryGetValue(codeKey, out var existingCode))
                {
                    // If code is not expired yet, wait a bit before allowing resend (rate limiting)
                    if (DateTime.UtcNow < existingCode.Expiry.AddMinutes(-9)) // Allow resend 1 minute after creation
                    {
                        var minutesRemaining = existingCode.Expiry.AddMinutes(-9).Minute;
                        return (false, $"Please wait {minutesRemaining} minutes before requesting a new code.", null);
                    }
                }
                else
                {
                    // No existing code found - user must use forgot password first
                    return (false, "Please use forgot password first.", null);
                }

                // Generate new reset code
                var newResetCode = GenerateVerificationCode();
                _verificationCodes[codeKey] = (newResetCode, DateTime.UtcNow.AddMinutes(10));

                // Send SMS
                var smsSent = await _smsService.SendPasswordResetCodeAsync(resendCodeDto.PhoneNumber, newResetCode);
                if (!smsSent)
                {
                    _logger.LogWarning("Failed to send password reset SMS to {PhoneNumber}", resendCodeDto.PhoneNumber);
                }

                return (true, "If the phone number exists, a new reset code has been sent.", newSessionToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during resend verification code for {PhoneNumber}", resendCodeDto.PhoneNumber);
                return (false, "An error occurred during resend verification code process.", null);
            }
        }

        /// <summary>
        /// Verifies the verification code for password reset
        /// </summary>
        public async Task<(bool Success, string Message, string? VerificationCode, string? SessionToken)> VerifyCodeAsync(VerifyCodeDto verifyCodeDto)
        {
            try
            {
                // Validate session first
                var sessionValidation = await ValidatePasswordResetSessionAsync(new PasswordResetSessionDto(verifyCodeDto.PhoneNumber, verifyCodeDto.SessionToken));
                if (!sessionValidation.Success)
                {
                    return (false, sessionValidation.Message, null, null);
                }

                // Remove old session and generate new one
                _passwordResetSessions.Remove(verifyCodeDto.SessionToken);
                var newSessionToken = GenerateSessionToken();
                _passwordResetSessions[newSessionToken] = (verifyCodeDto.PhoneNumber, DateTime.UtcNow.AddMinutes(10), "code_verified");

                // Find user
                var user = await _userManager.FindByNameAsync(verifyCodeDto.PhoneNumber);
                if (user == null)
                {
                    return (false, "User not found.", null, null);
                }

                // Check reset code
                var codeKey = $"{verifyCodeDto.PhoneNumber}_reset";
                if (!_verificationCodes.TryGetValue(codeKey, out var codeInfo))
                {
                    return (false, "Reset code not found or expired.", null, null);
                }

                if (DateTime.UtcNow > codeInfo.Expiry)
                {
                    _verificationCodes.Remove(codeKey);
                    return (false, "Reset code has expired.", null, null);
                }

                if (codeInfo.Code != verifyCodeDto.VerificationCode)
                {
                    return (false, "Invalid reset code.", null, null);
                }

                return (true, "Verification code is valid.", codeInfo.Code, newSessionToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during code verification for {PhoneNumber}", verifyCodeDto.PhoneNumber);
                return (false, "An error occurred during code verification.", null, null);
            }
        }

        /// <summary>
        /// Sets new password after verification code validation
        /// </summary>
        public async Task<(bool Success, string Message)> SetNewPasswordAsync(SetNewPasswordDto setNewPasswordDto)
        {
            try
            {
                // Validate session first
                var sessionValidation = await ValidatePasswordResetSessionAsync(new PasswordResetSessionDto(setNewPasswordDto.PhoneNumber, setNewPasswordDto.SessionToken));
                if (!sessionValidation.Success)
                {
                    return (false, sessionValidation.Message);
                }

                // Find user
                var user = await _userManager.FindByNameAsync(setNewPasswordDto.PhoneNumber);
                if (user == null)
                {
                    return (false, "User not found.");
                }

                // Check reset code again for security
                var codeKey = $"{setNewPasswordDto.PhoneNumber}_reset";
                if (!_verificationCodes.TryGetValue(codeKey, out var codeInfo))
                {
                    return (false, "Reset code not found or expired.");
                }

                if (DateTime.UtcNow > codeInfo.Expiry)
                {
                    _verificationCodes.Remove(codeKey);
                    return (false, "Reset code has expired.");
                }

                if (codeInfo.Code != setNewPasswordDto.VerificationCode)
                {
                    return (false, "Invalid reset code.");
                }

                // Reset password
                var token = await _userManager.GeneratePasswordResetTokenAsync(user);
                var result = await _userManager.ResetPasswordAsync(user, token, setNewPasswordDto.NewPassword);
                if (!result.Succeeded)
                {
                    var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                    return (false, $"Failed to reset password: {errors}");
                }

                // Remove reset code and session
                _verificationCodes.Remove(codeKey);

                // Remove all sessions for this phone number
                var sessionsToRemove = _passwordResetSessions.Where(kvp => kvp.Value.PhoneNumber == setNewPasswordDto.PhoneNumber)
                                                           .Select(kvp => kvp.Key)
                                                           .ToList();
                foreach (var sessionKey in sessionsToRemove)
                {
                    _passwordResetSessions.Remove(sessionKey);
                }

                // Revoke all refresh tokens for security
                await _refreshTokenService.RevokeAllUserTokensAsync(user.Id, "Password changed");

                return (true, "Password reset successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during password reset for {PhoneNumber}", setNewPasswordDto.PhoneNumber);
                return (false, "An error occurred during password reset.");
            }
        }

        /// <summary>
        /// Validates password reset session
        /// </summary>
        public async Task<(bool Success, string Message)> ValidatePasswordResetSessionAsync(PasswordResetSessionDto sessionDto)
        {
            try
            {
                // Check if session exists
                if (!_passwordResetSessions.TryGetValue(sessionDto.SessionToken, out var sessionInfo))
                {
                    return (false, "Invalid or expired session token.");
                }

                // Check if session is expired
                if (DateTime.UtcNow > sessionInfo.Expiry)
                {
                    _passwordResetSessions.Remove(sessionDto.SessionToken);
                    return (false, "Session has expired. Please start the password reset process again.");
                }

                // Check if phone number matches
                if (sessionInfo.PhoneNumber != sessionDto.PhoneNumber)
                {
                    return (false, "Session token does not match the phone number.");
                }

                return (true, "Session is valid.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during session validation for {PhoneNumber}", sessionDto.PhoneNumber);
                return (false, "An error occurred during session validation.");
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
            try
            {
                return await _refreshTokenService.RefreshTokenAsync(refreshToken);
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
            var random = new Random();
            return random.Next(100000, 999999).ToString();
        }

        /// <summary>
        /// Generates a secure session token
        /// </summary>
        private static string GenerateSessionToken()
        {
            var random = new Random();
            var bytes = new byte[32];
            random.NextBytes(bytes);
            return Convert.ToBase64String(bytes).Replace("+", "-").Replace("/", "_").Replace("=", "");
        }
    }
}
