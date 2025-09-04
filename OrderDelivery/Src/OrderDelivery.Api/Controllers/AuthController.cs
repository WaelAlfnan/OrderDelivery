using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OrderDelivery.Application.DTOs.Requests;
using OrderDelivery.Application.DTOs.Responses;
using OrderDelivery.Application.Interfaces;

namespace OrderDelivery.Api.Controllers
{
    /// <summary>
    /// Authentication controller for user login and account management
    /// </summary>
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly ILogger<AuthController> _logger;

        public AuthController(IAuthService authService, ILogger<AuthController> logger)
        {
            _authService = authService;
            _logger = logger;
        }

        /// <summary>
        /// Authenticates user and returns JWT token
        /// </summary>
        /// <param name="loginDto">User login data</param>
        /// <returns>Authentication response with JWT token</returns>
        [HttpPost("login")]
        [ProducesResponseType(typeof(ApiResponse<AuthResponseDto>), 200)]
        [ProducesResponseType(typeof(ApiResponse<object>), 400)]
        public async Task<IActionResult> Login([FromBody] LoginDto loginDto)
        {
            try
            {
                // Capture IP address for security tracking
                var ipAddress = GetClientIpAddress();

                var (success, message, authResponse) = await _authService.LoginAsync(loginDto);

                if (success)
                {
                    _logger.LogInformation("User logged in successfully from IP: {IpAddress}", ipAddress);
                    return Ok(new ApiResponse<AuthResponseDto>(authResponse, message, true));
                }

                _logger.LogWarning("Failed login attempt from IP: {IpAddress}", ipAddress);
                return BadRequest(new ApiResponse<object>(null, message, false));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in Login endpoint");
                return StatusCode(500, new ApiResponse<object>(null, "An internal server error occurred", false));
            }
        }

        /// <summary>
        /// Logs out user and revokes all refresh tokens
        /// </summary>
        /// <returns>Logout result</returns>
        [HttpPost("logout")]
        [Authorize]
        [ProducesResponseType(typeof(ApiResponse<object>), 200)]
        [ProducesResponseType(typeof(ApiResponse<object>), 401)]
        public async Task<IActionResult> Logout()
        {
            try
            {
                var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userId) || !Guid.TryParse(userId, out var userGuid))
                {
                    return Unauthorized(new ApiResponse<object>(null, "User not authenticated", false));
                }

                var (success, message) = await _authService.LogoutAsync(userGuid);

                if (success)
                {
                    return Ok(new ApiResponse<object>(null, message, true));
                }

                return BadRequest(new ApiResponse<object>(null, message, false));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in Logout endpoint");
                return StatusCode(500, new ApiResponse<object>(null, "An internal server error occurred", false));
            }
        }

        /// <summary>
        /// Initiates password reset process
        /// </summary>
        /// <param name="forgotPasswordDto">Forgot password data</param>
        /// <returns>Reset initiation result</returns>
        [HttpPost("forgot-password")]
        [ProducesResponseType(typeof(ApiResponse<object>), 200)]
        [ProducesResponseType(typeof(ApiResponse<object>), 400)]
        public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordDto forgotPasswordDto)
        {
            try
            {
                var (success, message, sessionToken) = await _authService.ForgotPasswordAsync(forgotPasswordDto);

                if (success)
                {
                    var responseData = new
                    {
                        sessionToken = sessionToken
                    };
                    return Ok(new ApiResponse<object>(responseData, message, true));
                }

                return Ok(new ApiResponse<object>(null, message, false));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in ForgotPassword endpoint");
                return StatusCode(500, new ApiResponse<object>(null, "An internal server error occurred", false));
            }
        }

        /// <summary>
        /// Resends verification code for password reset
        /// </summary>
        /// <param name="resendCodeDto">Resend code data</param>
        /// <returns>Resend code result</returns>
        [HttpPost("resend-code")]
        [ProducesResponseType(typeof(ApiResponse<object>), 200)]
        [ProducesResponseType(typeof(ApiResponse<object>), 400)]
        public async Task<IActionResult> ResendVerificationCode([FromBody] ResendCodeDto resendCodeDto)
        {
            try
            {
                var (success, message, sessionToken) = await _authService.ResendVerificationCodeAsync(resendCodeDto);

                if (success)
                {
                    var responseData = new
                    {
                        sessionToken = sessionToken
                    };
                    return Ok(new ApiResponse<object>(responseData, message, true));
                }

                return BadRequest(new ApiResponse<object>(null, message, false));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in ResendVerificationCode endpoint");
                return StatusCode(500, new ApiResponse<object>(null, "An internal server error occurred", false));
            }
        }

        /// <summary>
        /// Verifies the verification code for password reset
        /// </summary>
        /// <param name="verifyCodeDto">Verification code data</param>
        /// <returns>Code verification result</returns>
        [HttpPost("verify-code")]
        [ProducesResponseType(typeof(ApiResponse<object>), 200)]
        [ProducesResponseType(typeof(ApiResponse<object>), 400)]
        public async Task<IActionResult> VerifyCode([FromBody] VerifyCodeDto verifyCodeDto)
        {
            try
            {
                var (success, message, verificationCode, sessionToken) = await _authService.VerifyCodeAsync(verifyCodeDto);

                if (success)
                {
                    var responseData = new
                    {
                        verificationCode = verificationCode,
                        sessionToken = sessionToken
                    };
                    return Ok(new ApiResponse<object>(responseData, message, true));
                }

                return BadRequest(new ApiResponse<object>(null, message, false));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in VerifyCode endpoint");
                return StatusCode(500, new ApiResponse<object>(null, "An internal server error occurred", false));
            }
        }

        /// <summary>
        /// Sets new password after verification code validation
        /// </summary>
        /// <param name="setNewPasswordDto">New password data</param>
        /// <returns>Password reset result</returns>
        [HttpPost("set-new-password")]
        [ProducesResponseType(typeof(ApiResponse<object>), 200)]
        [ProducesResponseType(typeof(ApiResponse<object>), 400)]
        public async Task<IActionResult> SetNewPassword([FromBody] SetNewPasswordDto setNewPasswordDto)
        {
            try
            {
                var (success, message) = await _authService.SetNewPasswordAsync(setNewPasswordDto);

                if (success)
                {
                    return Ok(new ApiResponse<object>(null, message, true));
                }

                return BadRequest(new ApiResponse<object>(null, message, false));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in SetNewPassword endpoint");
                return StatusCode(500, new ApiResponse<object>(null, "An internal server error occurred", false));
            }
        }

        /// <summary>
        /// Gets current user profile
        /// </summary>
        /// <returns>User profile information</returns>
        [HttpGet("profile")]
        [Authorize]
        [ProducesResponseType(typeof(ApiResponse<UserDto>), 200)]
        [ProducesResponseType(typeof(ApiResponse<object>), 401)]
        [ProducesResponseType(typeof(ApiResponse<object>), 404)]
        public async Task<IActionResult> GetProfile()
        {
            try
            {
                var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userId))
                {
                    return Unauthorized(new ApiResponse<object>(null, "User not authenticated", false));
                }

                var (success, message, user) = await _authService.GetUserProfileAsync(userId);

                if (success)
                {
                    return Ok(new ApiResponse<UserDto>(user, message, true));
                }

                return NotFound(new ApiResponse<object>(null, message, false));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in GetProfile endpoint");
                return StatusCode(500, new ApiResponse<object>(null, "An internal server error occurred", false));
            }
        }

        /// <summary>
        /// Refreshes JWT token
        /// </summary>
        /// <param name="refreshTokenRequest">Refresh token request</param>
        /// <returns>New authentication response</returns>
        [HttpPost("refresh-token")]
        [ProducesResponseType(typeof(ApiResponse<AuthResponseDto>), 200)]
        [ProducesResponseType(typeof(ApiResponse<object>), 400)]
        public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenRequest refreshTokenRequest)
        {
            try
            {
                if (string.IsNullOrEmpty(refreshTokenRequest.RefreshToken))
                {
                    return BadRequest(new ApiResponse<object>(null, "Refresh token is required", false));
                }

                // Capture IP address for security tracking
                var ipAddress = GetClientIpAddress();

                var (success, message, authResponse) = await _authService.RefreshTokenAsync(refreshTokenRequest.RefreshToken);

                if (success)
                {
                    _logger.LogInformation("Token refreshed successfully from IP: {IpAddress}", ipAddress);
                    return Ok(new ApiResponse<AuthResponseDto>(authResponse, message, true));
                }

                _logger.LogWarning("Failed token refresh attempt from IP: {IpAddress}", ipAddress);
                return BadRequest(new ApiResponse<object>(null, message, false));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in RefreshToken endpoint");
                return StatusCode(500, new ApiResponse<object>(null, "An internal server error occurred", false));
            }
        }

        /// <summary>
        /// Gets the client IP address from the request
        /// </summary>
        /// <returns>Client IP address</returns>
        private string GetClientIpAddress()
        {
            // Try to get IP from X-Forwarded-For header (for proxy/load balancer scenarios)
            var forwardedHeader = Request.Headers["X-Forwarded-For"].FirstOrDefault();
            if (!string.IsNullOrEmpty(forwardedHeader))
            {
                // X-Forwarded-For can contain multiple IPs, take the first one
                var firstIp = forwardedHeader.Split(',')[0].Trim();
                if (!string.IsNullOrEmpty(firstIp))
                {
                    return firstIp;
                }
            }

            // Try to get IP from X-Real-IP header
            var realIp = Request.Headers["X-Real-IP"].FirstOrDefault();
            if (!string.IsNullOrEmpty(realIp))
            {
                return realIp;
            }

            // Fallback to connection remote IP
            var remoteIp = HttpContext.Connection.RemoteIpAddress?.ToString();
            return remoteIp ?? "Unknown";
        }
    }
}