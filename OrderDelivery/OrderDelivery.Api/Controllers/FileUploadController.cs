using Microsoft.AspNetCore.Mvc;
using OrderDelivery.Application.DTOs.Requests;
using OrderDelivery.Application.Interfaces;
using OrderDelivery.Application.Validators;

namespace OrderDelivery.Api.Controllers
{

    [ApiController]
    [Route("api/[controller]")]
    public class FileUploadController : ControllerBase
    {
        private readonly IFileStorageService _fileStorageService;
        private readonly UploadPhotoDtoValidator _validator;
        private readonly ILogger<FileUploadController> _logger;

        public FileUploadController(
            IFileStorageService fileStorageService,
            UploadPhotoDtoValidator validator,
            ILogger<FileUploadController> logger)
        {
            _fileStorageService = fileStorageService;
            _validator = validator;
            _logger = logger;
        }

        [HttpPost("upload-photo")]
        public async Task<IActionResult> UploadPhoto([FromForm] UploadPhotoDto uploadDto)
        {
            try
            {
                // Validate the upload request
                var validationResult = await _validator.ValidateAsync(uploadDto);
                if (!validationResult.IsValid)
                {
                    return BadRequest(new
                    {
                        Message = "Validation failed",
                        Errors = validationResult.Errors.Select(e => e.ErrorMessage)
                    });
                }

                // Upload photo to S3
                var result = await _fileStorageService.UploadPhotoAsync(uploadDto);

                _logger.LogInformation("Photo uploaded successfully. FileName: {FileName}, URL: {Url}",
                    result.FileName, result.FileUrl);

                return Ok(new
                {
                    Message = "Photo uploaded successfully",
                    Data = result
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error uploading photo. FileName: {FileName}",
                    uploadDto.File?.FileName ?? "Unknown");

                return StatusCode(500, new
                {
                    Message = "An error occurred while uploading the photo",
                    Error = ex.Message
                });
            }
        }

        [HttpPost("upload-photo-with-folder")]
        public async Task<IActionResult> UploadPhotoWithFolder([FromForm] UploadPhotoDto uploadDto)
        {
            try
            {
                // Validate the upload request
                var validationResult = await _validator.ValidateAsync(uploadDto);
                if (!validationResult.IsValid)
                {
                    return BadRequest(new
                    {
                        Message = "Validation failed",
                        Errors = validationResult.Errors.Select(e => e.ErrorMessage)
                    });
                }

                // Upload photo to S3
                var result = await _fileStorageService.UploadPhotoAsync(uploadDto);

                _logger.LogInformation("Photo uploaded successfully to folder {Folder}. FileName: {FileName}, URL: {Url}",
                    uploadDto.FolderName, result.FileName, result.FileUrl);

                return Ok(new
                {
                    Message = "Photo uploaded successfully",
                    Data = result
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error uploading photo to folder {Folder}. FileName: {FileName}",
                    uploadDto.FolderName, uploadDto.File?.FileName ?? "Unknown");

                return StatusCode(500, new
                {
                    Message = "An error occurred while uploading the photo",
                    Error = ex.Message
                });
            }
        }

        [HttpDelete("delete-photo")]
        public async Task<IActionResult> DeletePhoto([FromQuery] string fileUrl)
        {
            try
            {
                if (string.IsNullOrEmpty(fileUrl))
                {
                    return BadRequest(new { Message = "File URL is required" });
                }

                var isDeleted = await _fileStorageService.DeletePhotoAsync(fileUrl);

                if (isDeleted)
                {
                    _logger.LogInformation("Photo deleted successfully. URL: {Url}", fileUrl);
                    return Ok(new { Message = "Photo deleted successfully" });
                }
                else
                {
                    return NotFound(new { Message = "Photo not found or could not be deleted" });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting photo. URL: {Url}", fileUrl);

                return StatusCode(500, new
                {
                    Message = "An error occurred while deleting the photo",
                    Error = ex.Message
                });
            }
        }

        [HttpGet("check-photo-exists")]
        public async Task<IActionResult> CheckPhotoExists([FromQuery] string fileUrl)
        {
            try
            {
                if (string.IsNullOrEmpty(fileUrl))
                {
                    return BadRequest(new { Message = "File URL is required" });
                }

                var exists = await _fileStorageService.PhotoExistsAsync(fileUrl);

                return Ok(new
                {
                    Exists = exists,
                    Message = exists ? "Photo exists" : "Photo does not exist"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking if photo exists. URL: {Url}", fileUrl);

                return StatusCode(500, new
                {
                    Message = "An error occurred while checking photo existence",
                    Error = ex.Message
                });
            }
        }

        [HttpGet("test-connection")]
        public async Task<IActionResult> TestConnection()
        {
            try
            {
                var isConnected = await _fileStorageService.TestConnectionAsync();

                if (isConnected)
                {
                    return Ok(new
                    {
                        Message = "S3 connection successful",
                        Status = "Connected"
                    });
                }
                else
                {
                    return StatusCode(500, new
                    {
                        Message = "S3 connection failed",
                        Status = "Disconnected"
                    });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error testing S3 connection");

                return StatusCode(500, new
                {
                    Message = "An error occurred while testing S3 connection",
                    Error = ex.Message
                });
            }
        }
    }

}
