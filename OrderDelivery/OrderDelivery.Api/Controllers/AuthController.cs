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
    [ApiController]
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
                if (!ModelState.IsValid)
                {
                    return BadRequest(new ApiResponse<object>(
                        ModelState.Values.SelectMany(v => v.Errors.Select(e => e.ErrorMessage)),
                        "Invalid input data",
                        false
                    ));
                }

                var (success, message, authResponse) = await _authService.LoginAsync(loginDto);

                if (success)
                {
                    return Ok(new ApiResponse<AuthResponseDto>(authResponse, message, true));
                }

                return BadRequest(new ApiResponse<object>(null, message, false));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in Login endpoint");
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
                if (!ModelState.IsValid)
                {
                    return BadRequest(new ApiResponse<object>(
                        ModelState.Values.SelectMany(v => v.Errors.Select(e => e.ErrorMessage)),
                        "Invalid input data",
                        false
                    ));
                }

                var (success, message) = await _authService.ForgotPasswordAsync(forgotPasswordDto);

                return Ok(new ApiResponse<object>(null, message, success));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in ForgotPassword endpoint");
                return StatusCode(500, new ApiResponse<object>(null, "An internal server error occurred", false));
            }
        }

        /// <summary>
        /// Resets password with verification code
        /// </summary>
        /// <param name="resetPasswordDto">Password reset data</param>
        /// <returns>Password reset result</returns>
        [HttpPost("reset-password")]
        [ProducesResponseType(typeof(ApiResponse<object>), 200)]
        [ProducesResponseType(typeof(ApiResponse<object>), 400)]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordDto resetPasswordDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(new ApiResponse<object>(
                        ModelState.Values.SelectMany(v => v.Errors.Select(e => e.ErrorMessage)),
                        "Invalid input data",
                        false
                    ));
                }

                var (success, message) = await _authService.ResetPasswordAsync(resetPasswordDto);

                if (success)
                {
                    return Ok(new ApiResponse<object>(null, message, true));
                }

                return BadRequest(new ApiResponse<object>(null, message, false));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in ResetPassword endpoint");
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
        /// <param name="refreshToken">Refresh token</param>
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

                var (success, message, authResponse) = await _authService.RefreshTokenAsync(refreshTokenRequest.RefreshToken);

                if (success)
                {
                    return Ok(new ApiResponse<AuthResponseDto>(authResponse, message, true));
                }

                return BadRequest(new ApiResponse<object>(null, message, false));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in RefreshToken endpoint");
                return StatusCode(500, new ApiResponse<object>(null, "An internal server error occurred", false));
            }
        }
    }
}