using Microsoft.AspNetCore.Http;

namespace OrderDelivery.Application.DTOs.RegistrationSteps;

/// <summary>
/// DTO for setting basic user information during registration
/// Photos are provided as IFormFile for direct S3 upload (all photos are mandatory)
/// </summary>
public record SetBasicInfoDto(
    string PhoneNumber,
    string FullName,
    IFormFile PersonalPhoto,        // Photo file for upload (required)
    IFormFile NationalIdFrontPhoto, // National ID front photo (required)
    IFormFile NationalIdBackPhoto,  // National ID back photo (required)
    string NationalIdNumber
);
