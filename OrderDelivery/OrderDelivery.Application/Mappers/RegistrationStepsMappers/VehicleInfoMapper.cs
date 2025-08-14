using OrderDelivery.Application.DataModels.RegistrationSteps;
using OrderDelivery.Application.DTOs.RegistrationSteps;
using OrderDelivery.Domain.Entities;

namespace OrderDelivery.Application.Mappers.RegistrationStepsMappers;

public static class VehicleInfoMapper
{
    /// <summary>
    /// Maps VehicleInfoDto to VehicleInfoData
    /// Note: Photos are handled separately in the service layer
    /// </summary>
    /// <param name="dto">The DTO containing VehicleInfo</param>
    /// <returns>VehicleInfoData object</returns>
    public static VehicleInfoData ToVehicleInfoData(this VehicleInfoDto dto)
    {
        if (dto == null)
            throw new ArgumentNullException(nameof(dto));

        return new VehicleInfoData
        {
            VehicleBrand = dto.VehicleBrand ?? string.Empty,
            VehiclePlateNumber = dto.VehiclePlateNumber ?? string.Empty,
            VehicleIssueDate = dto.VehicleIssueDate,
            ShasehPhotoUrl = string.Empty,    // Will be set by service after S3 upload
            EfragPhotoUrl = string.Empty      // Will be set by service after S3 upload
        };
    }

    /// <summary>
    /// Maps VehicleInfoData to VehicleInfoDto
    /// Useful for updating existing vehicle info
    /// Note: Photos are not included in DTO for updates since they are mandatory for new registrations
    /// </summary>
    /// <param name="data">The VehicleInfoData object</param>
    /// <param name="phoneNumber">Phone number for the DTO</param>
    /// <returns>VehicleInfoDto object</returns>
    public static VehicleInfoDto ToVehicleInfoDto(this VehicleInfoData data, string phoneNumber)
    {
        if (data == null)
            throw new ArgumentNullException(nameof(data));

        return new VehicleInfoDto(
            PhoneNumber: phoneNumber,
            VehicleBrand: data.VehicleBrand,
            VehiclePlateNumber: data.VehiclePlateNumber,
            VehicleIssueDate: data.VehicleIssueDate,
            ShasehPhoto: null!,               // Photos are not included in DTO for updates
            EfragPhoto: null!                 // Photos are not included in DTO for updates
        );
    }

    /// <summary>
    /// Maps VehicleInfoData to Vehicle entity
    /// </summary>
    /// <param name="vehicleInfo">The VehicleInfoData containing vehicle information</param>
    /// <param name="driverId">The Driver ID to associate with the vehicle</param>
    /// <returns>Vehicle entity object</returns>
    public static Vehicle ToVehicle(this VehicleInfoData vehicleInfo, Guid driverId)
    {
        if (vehicleInfo == null)
            throw new ArgumentNullException(nameof(vehicleInfo));

        return new Vehicle
        {
            DriverId = driverId,
            VehicleBrand = vehicleInfo.VehicleBrand,
            VehiclePlateNumber = vehicleInfo.VehiclePlateNumber,
            VehicleIssueDate = vehicleInfo.VehicleIssueDate,
            ShasehPhotoUrl = vehicleInfo.ShasehPhotoUrl,
            EfragPhotoUrl = vehicleInfo.EfragPhotoUrl
        };
    }
}