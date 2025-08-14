using OrderDelivery.Application.DataModels.RegistrationSteps;
using OrderDelivery.Application.DTOs.RegistrationSteps;

namespace OrderDelivery.Application.Mappers.RegistrationStepsMappers;

public static class BasicInfoMapper
{
    /// <summary>
    /// Maps SetBasicInfoDto to BasicInfoData
    /// Note: Photos are handled separately in the service layer
    /// </summary>
    /// <param name="dto">The DTO containing basic info</param>
    /// <returns>BasicInfoData object</returns>
    public static BasicInfoData ToBasicInfoData(this SetBasicInfoDto dto)
    {
        if (dto == null)
            throw new ArgumentNullException(nameof(dto));

        return new BasicInfoData
        {
            FullName = dto.FullName ?? string.Empty,
            PersonalPhotoUrl = string.Empty,        // Will be set by service after S3 upload
            NationalIdFrontPhotoUrl = string.Empty, // Will be set by service after S3 upload
            NationalIdBackPhotoUrl = string.Empty,  // Will be set by service after S3 upload
            NationalIdNumber = dto.NationalIdNumber ?? string.Empty
        };
    }

    /// <summary>
    /// Maps BasicInfoData to SetBasicInfoDto
    /// Useful for updating existing basic info
    /// Note: Photos are not included in DTO for updates since they are mandatory for new registrations
    /// </summary>
    /// <param name="data">The BasicInfoData object</param>
    /// <param name="phoneNumber">Phone number for the DTO</param>
    /// <returns>SetBasicInfoDto object</returns>
    public static SetBasicInfoDto ToSetBasicInfoDto(this BasicInfoData data, string phoneNumber)
    {
        if (data == null)
            throw new ArgumentNullException(nameof(data));

        return new SetBasicInfoDto(
            PhoneNumber: phoneNumber,
            FullName: data.FullName,
            PersonalPhoto: null!,              // Photos are not included in DTO for updates
            NationalIdFrontPhoto: null!,       // Photos are not included in DTO for updates
            NationalIdBackPhoto: null!,        // Photos are not included in DTO for updates
            NationalIdNumber: data.NationalIdNumber
        );
    }
}

