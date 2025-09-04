using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using OrderDelivery.Application.DTOs.Responses;
using OrderDelivery.Application.Interfaces;
using OrderDelivery.Application.Mappers.RegistrationStepsMappers;
using OrderDelivery.Domain.Entities;

namespace OrderDelivery.Application.Services
{
    /// <summary>
    /// Service for managing refresh tokens
    /// </summary>
    public class RefreshTokenService : IRefreshTokenService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IJwtService _jwtService;
        private readonly ILogger<RefreshTokenService> _logger;

        public RefreshTokenService(
            UserManager<ApplicationUser> userManager,
            IJwtService jwtService,
            ILogger<RefreshTokenService> logger)
        {
            _userManager = userManager;
            _jwtService = jwtService;
            _logger = logger;
        }

        /// <summary>
        /// Creates a new refresh token for a user
        /// </summary>
        public async Task<string> CreateRefreshTokenAsync(Guid userId, string? ipAddress = null)
        {
            var user = await _userManager.FindByIdAsync(userId.ToString());
            if (user == null)
            {
                throw new InvalidOperationException("User not found");
            }

            // Generate new refresh token
            var refreshTokenString = _jwtService.GenerateRefreshToken();

            // Create refresh token entity with enhanced security
            var refreshToken = new RefreshToken
            {
                Id = Guid.NewGuid(),
                Token = refreshTokenString,
                ExpiresOn = DateTime.UtcNow.AddDays(7), // 7 days expiry
                CreatedOn = DateTime.UtcNow,
                CreatedByIp = ipAddress,
                ApplicationUserId = userId
            };

            // Add to user's refresh tokens
            user.RefreshTokens.Add(refreshToken);

            // Limit the number of refresh tokens per user (keep only last 5)
            if (user.RefreshTokens.Count > 5)
            {
                var oldestTokens = user.RefreshTokens
                    .OrderBy(rt => rt.CreatedOn)
                    .Take(user.RefreshTokens.Count - 5);

                foreach (var oldToken in oldestTokens)
                {
                    user.RefreshTokens.Remove(oldToken);
                }
            }

            await _userManager.UpdateAsync(user);
            _logger.LogInformation("Created refresh token for user {UserId} from IP {IpAddress}", userId, ipAddress ?? "Unknown");
            return refreshTokenString;
        }

        /// <summary>
        /// Validates and refreshes a token
        /// </summary>
        public async Task<(bool Success, string Message, AuthResponseDto? AuthResponse)> RefreshTokenAsync(string refreshToken)
        {
            try
            {
                // Find user with this refresh token
                var user = await _userManager.Users
                    .FirstOrDefaultAsync(u => u.RefreshTokens.Any(rt => rt.Token == refreshToken));

                if (user == null)
                {
                    return (false, "Invalid refresh token.", null);
                }

                // Get the specific refresh token
                var tokenEntity = user.RefreshTokens.First(rt => rt.Token == refreshToken);

                // Check if token is valid
                if (!tokenEntity.IsActive)
                {
                    return (false, "Refresh token is expired or revoked.", null);
                }

                // Get user roles
                var roles = await _userManager.GetRolesAsync(user);

                // Create user DTO
                var userDto = user.ToUserDto(roles.FirstOrDefault() ?? string.Empty);

                // Generate new access token
                var newAccessToken = _jwtService.GenerateAccessToken(user.Id.ToString(), userDto, roles.ToList());

                // Generate new refresh token
                var newRefreshToken = await CreateRefreshTokenAsync(user.Id, null); // IP will be captured in the calling method

                // Revoke the old refresh token
                await RevokeTokenAsync(refreshToken, "Token refreshed");

                var authResponse = new AuthResponseDto(
                    AccessToken: newAccessToken,
                    RefreshToken: newRefreshToken,
                    ExpiresAt: DateTime.UtcNow.AddMinutes(60),
                    User: userDto
                );

                return (true, "Token refreshed successfully.", authResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during token refresh");
                return (false, "An error occurred during token refresh.", null);
            }
        }

        /// <summary>
        /// Revokes all refresh tokens for a user
        /// </summary>
        public async Task<bool> RevokeAllUserTokensAsync(Guid userId, string reason)
        {
            try
            {
                var user = await _userManager.FindByIdAsync(userId.ToString());
                if (user == null)
                {
                    return false;
                }

                foreach (var token in user.RefreshTokens)
                {
                    token.RevokedOn = DateTime.UtcNow;
                }

                await _userManager.UpdateAsync(user);
                _logger.LogInformation("Revoked all refresh tokens for user {UserId}. Reason: {Reason}", userId, reason);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error revoking all tokens for user {UserId}", userId);
                return false;
            }
        }

        /// <summary>
        /// Revokes a specific refresh token
        /// </summary>
        public async Task<bool> RevokeTokenAsync(string refreshToken, string reason, string? ipAddress = null)
        {
            try
            {
                var user = await _userManager.Users
                    .FirstOrDefaultAsync(u => u.RefreshTokens.Any(rt => rt.Token == refreshToken));

                if (user == null)
                {
                    return false;
                }

                var tokenEntity = user.RefreshTokens.First(rt => rt.Token == refreshToken);
                tokenEntity.RevokedOn = DateTime.UtcNow;
                tokenEntity.RevokedReason = reason;
                tokenEntity.RevokedByIp = ipAddress;

                await _userManager.UpdateAsync(user);
                _logger.LogInformation("Revoked refresh token for user {UserId}. Reason: {Reason}, IP: {IpAddress}",
                    user.Id, reason, ipAddress ?? "Unknown");
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error revoking token {Token}", refreshToken);
                return false;
            }
        }

        /// <summary>
        /// Revokes a specific refresh token (alias for RevokeTokenAsync)
        /// </summary>
        public async Task<bool> RevokeRefreshTokenAsync(string refreshToken)
        {
            return await RevokeTokenAsync(refreshToken, "Token revoked");
        }

        /// <summary>
        /// Revokes all refresh tokens for a user (alias for RevokeAllUserTokensAsync)
        /// </summary>
        public async Task<bool> RevokeAllRefreshTokensAsync(Guid userId)
        {
            return await RevokeAllUserTokensAsync(userId, "All tokens revoked");
        }

        /// <summary>
        /// Validates if a refresh token is still valid
        /// </summary>
        public async Task<bool> IsRefreshTokenValidAsync(string refreshToken)
        {
            try
            {
                var user = await _userManager.Users
                    .FirstOrDefaultAsync(u => u.RefreshTokens.Any(rt => rt.Token == refreshToken));

                if (user == null)
                {
                    return false;
                }

                var tokenEntity = user.RefreshTokens.First(rt => rt.Token == refreshToken);
                return tokenEntity.IsActive;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error validating refresh token");
                return false;
            }
        }
    }
}
