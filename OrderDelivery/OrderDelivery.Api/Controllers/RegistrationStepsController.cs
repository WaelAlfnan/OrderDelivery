using Microsoft.AspNetCore.Mvc;
using OrderDelivery.Application.DTOs.RegistrationSteps;
using OrderDelivery.Application.DTOs.Responses;
using OrderDelivery.Application.Interfaces;
using OrderDelivery.Domain.Entities;

namespace OrderDelivery.Api.Controllers
{
    /// <summary>
    /// Controller for handling multi-step registration process
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class RegistrationStepsController : ControllerBase
    {
        private readonly IRegistrationStepsService _registrationStepsService;
        private readonly ILogger<RegistrationStepsController> _logger;

        public RegistrationStepsController(
            IRegistrationStepsService registrationStepsService,
            ILogger<RegistrationStepsController> logger)
        {
            _registrationStepsService = registrationStepsService;
            _logger = logger;
        }

        /// <summary>
        /// Starts the registration process by registering phone number
        /// </summary>
        [HttpPost("start-registration")]
        [ProducesResponseType(typeof(ApiResponse<object>), 200)]
        [ProducesResponseType(typeof(ApiResponse<object>), 400)]
        public async Task<IActionResult> StartRegistration([FromBody] RegisterPhoneDto dto)
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

                var success = await _registrationStepsService.StartRegistrationAsync(dto);

                if (success)
                {
                    return Ok(new ApiResponse<object>(null, "Registration started successfully", true));
                }

                return BadRequest(new ApiResponse<object>(null, "Phone number already registered", false));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in StartRegistration endpoint");
                return StatusCode(500, new ApiResponse<object>(null, "An internal server error occurred", false));
            }
        }

        /// <summary>
        /// Verifies phone number with verification code
        /// </summary>
        [HttpPost("verify-phone")]
        [ProducesResponseType(typeof(ApiResponse<object>), 200)]
        [ProducesResponseType(typeof(ApiResponse<object>), 400)]
        public async Task<IActionResult> VerifyPhone([FromBody] VerifyPhoneDto dto)
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

                var success = await _registrationStepsService.VerifyPhoneAsync(dto);

                if (success)
                {
                    return Ok(new ApiResponse<object>(null, "Phone verified successfully", true));
                }

                return BadRequest(new ApiResponse<object>(null, "Invalid verification code or phone not found", false));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in VerifyPhone endpoint");
                return StatusCode(500, new ApiResponse<object>(null, "An internal server error occurred", false));
            }
        }

        /// <summary>
        /// Sets user password after phone verification
        /// </summary>
        [HttpPost("set-password")]
        [ProducesResponseType(typeof(ApiResponse<object>), 200)]
        [ProducesResponseType(typeof(ApiResponse<object>), 400)]
        public async Task<IActionResult> SetPassword([FromBody] SetPasswordDto dto)
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

                var success = await _registrationStepsService.SetPasswordAsync(dto);

                if (success)
                {
                    return Ok(new ApiResponse<object>(null, "Password set successfully", true));
                }

                return BadRequest(new ApiResponse<object>(null, "Failed to set password", false));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in SetPassword endpoint");
                return StatusCode(500, new ApiResponse<object>(null, "An internal server error occurred", false));
            }
        }

        /// <summary>
        /// Sets user role (Merchant or Driver)
        /// </summary>
        [HttpPost("set-role")]
        [ProducesResponseType(typeof(ApiResponse<object>), 200)]
        [ProducesResponseType(typeof(ApiResponse<object>), 400)]
        public async Task<IActionResult> SetRole([FromBody] SetRoleDto dto)
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

                var success = await _registrationStepsService.SetRoleAsync(dto);

                if (success)
                {
                    return Ok(new ApiResponse<object>(null, "Role set successfully", true));
                }

                return BadRequest(new ApiResponse<object>(null, "Failed to set role", false));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in SetRole endpoint");
                return StatusCode(500, new ApiResponse<object>(null, "An internal server error occurred", false));
            }
        }

        /// <summary>
        /// Sets basic user information after role selection
        /// </summary>
        [HttpPost("set-basic-info")]
        [ProducesResponseType(typeof(ApiResponse<object>), 200)]
        [ProducesResponseType(typeof(ApiResponse<object>), 400)]
        public async Task<IActionResult> SetBasicInfo([FromForm] SetBasicInfoDto dto)
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

                var success = await _registrationStepsService.SetBasicInfoAsync(dto);

                if (success)
                {
                    return Ok(new ApiResponse<object>(null, "Basic info set successfully", true));
                }

                return BadRequest(new ApiResponse<object>(null, "Failed to set basic info", false));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in SetBasicInfo endpoint");
                return StatusCode(500, new ApiResponse<object>(null, "An internal server error occurred", false));
            }
        }

        /// <summary>
        /// Sets merchant information
        /// </summary>
        [HttpPost("set-merchant-info")]
        [ProducesResponseType(typeof(ApiResponse<object>), 200)]
        [ProducesResponseType(typeof(ApiResponse<object>), 400)]
        public async Task<IActionResult> SetMerchantInfo([FromBody] MerchantInfoDto dto)
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

                var success = await _registrationStepsService.SetMerchantInfoAsync(dto);

                if (success)
                {
                    return Ok(new ApiResponse<object>(null, "Merchant info set successfully", true));
                }

                return BadRequest(new ApiResponse<object>(null, "Failed to set merchant info", false));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in SetMerchantInfo endpoint");
                return StatusCode(500, new ApiResponse<object>(null, "An internal server error occurred", false));
            }
        }

        /// <summary>
        /// Sets driver information
        /// </summary>
        [HttpPost("set-driver-info")]
        [ProducesResponseType(typeof(ApiResponse<object>), 200)]
        [ProducesResponseType(typeof(ApiResponse<object>), 400)]
        public async Task<IActionResult> SetDriverInfo([FromBody] DriverInfoDto dto)
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

                var success = await _registrationStepsService.SetDriverInfoAsync(dto);

                if (success)
                {
                    return Ok(new ApiResponse<object>(null, "Driver info set successfully", true));
                }

                return BadRequest(new ApiResponse<object>(null, "Failed to set driver info", false));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in SetDriverInfo endpoint");
                return StatusCode(500, new ApiResponse<object>(null, "An internal server error occurred", false));
            }
        }

        /// <summary>
        /// Sets vehicle information
        /// </summary>
        [HttpPost("set-vehicle-info")]
        [ProducesResponseType(typeof(ApiResponse<object>), 200)]
        [ProducesResponseType(typeof(ApiResponse<object>), 400)]
        public async Task<IActionResult> SetVehicleInfo([FromForm] VehicleInfoDto dto)
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

                var success = await _registrationStepsService.SetVehicleInfoAsync(dto);

                if (success)
                {
                    return Ok(new ApiResponse<object>(null, "Vehicle info set successfully", true));
                }

                return BadRequest(new ApiResponse<object>(null, "Failed to set vehicle info", false));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in SetVehicleInfo endpoint");
                return StatusCode(500, new ApiResponse<object>(null, "An internal server error occurred", false));
            }
        }

        /// <summary>
        /// Sets residence information
        /// </summary>
        [HttpPost("set-residence-info")]
        [ProducesResponseType(typeof(ApiResponse<object>), 200)]
        [ProducesResponseType(typeof(ApiResponse<object>), 400)]
        public async Task<IActionResult> SetResidenceInfo([FromBody] ResidenceInfoDto dto)
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

                var success = await _registrationStepsService.SetResidenceInfoAsync(dto);

                if (success)
                {
                    return Ok(new ApiResponse<object>(null, "Residence info set successfully", true));
                }

                return BadRequest(new ApiResponse<object>(null, "Failed to set residence info", false));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in SetResidenceInfo endpoint");
                return StatusCode(500, new ApiResponse<object>(null, "An internal server error occurred", false));
            }
        }

        /// <summary>
        /// Gets pending registration by phone number
        /// </summary>
        [HttpGet("pending-registration/{phoneNumber}")]
        [ProducesResponseType(typeof(ApiResponse<PendingRegistration>), 200)]
        [ProducesResponseType(typeof(ApiResponse<object>), 404)]
        public async Task<IActionResult> GetPendingRegistration(string phoneNumber)
        {
            try
            {
                var pendingRegistration = await _registrationStepsService.GetPendingRegistrationByPhoneAsync(phoneNumber);

                if (pendingRegistration != null)
                {
                    return Ok(new ApiResponse<PendingRegistration>(pendingRegistration, "Pending registration found", true));
                }

                return NotFound(new ApiResponse<object>(null, "Pending registration not found", false));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in GetPendingRegistration endpoint");
                return StatusCode(500, new ApiResponse<object>(null, "An internal server error occurred", false));
            }
        }

        /// <summary>
        /// Completes the registration process
        /// </summary>
        [HttpPost("complete-registration")]
        [ProducesResponseType(typeof(ApiResponse<object>), 200)]
        [ProducesResponseType(typeof(ApiResponse<object>), 400)]
        public async Task<IActionResult> CompleteRegistration([FromBody] CompleteRegistrationRequest request)
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

                var success = await _registrationStepsService.CompleteRegistrationAsync(request.PhoneNumber);

                if (success)
                {
                    return Ok(new ApiResponse<object>(null, "Registration completed successfully", true));
                }

                return BadRequest(new ApiResponse<object>(null, "Failed to complete registration", false));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in CompleteRegistration endpoint");
                return StatusCode(500, new ApiResponse<object>(null, "An internal server error occurred", false));
            }
        }
    }

    public record CompleteRegistrationRequest(string PhoneNumber);
}