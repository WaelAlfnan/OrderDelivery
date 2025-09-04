using Microsoft.AspNetCore.Http;

namespace OrderDelivery.Application.DTOs.RegistrationSteps;

/// <summary>
/// DTO for setting vehicle information during driver registration
/// Photos are provided as IFormFile for direct S3 upload (all photos are mandatory)
/// </summary>
public record VehicleInfoDto(
    string PhoneNumber,
    string VehicleBrand,
    string? VehiclePlateNumber,
    string VehicleIssueDate,
    IFormFile ShasehPhoto,    // Vehicle shaseh photo (required)
    IFormFile EfragPhoto      // Vehicle efrag photo (required)
);