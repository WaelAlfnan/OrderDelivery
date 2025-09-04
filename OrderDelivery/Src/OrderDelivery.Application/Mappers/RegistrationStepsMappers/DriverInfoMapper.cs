using OrderDelivery.Application.DataModels.RegistrationSteps;
using OrderDelivery.Application.DTOs.RegistrationSteps;
using OrderDelivery.Domain.Entities;
using OrderDelivery.Domain.Enums;

namespace OrderDelivery.Application.Mappers.RegistrationStepsMappers;

public static class DriverInfoMapper
{
    /// <summary>
    /// Maps DriverInfoDto to DriverInfoData
    /// </summary>
    /// <param name="dto">The DTO containing DriverInfo</param>
    /// <returns>DriverInfoData object</returns>
    public static DriverInfoData ToDriverInfoData(this DriverInfoDto dto)
    {
        if (dto == null)
            throw new ArgumentNullException(nameof(dto));

        return new DriverInfoData
        {
            VehicleType = dto.VehicleType,
            IsAvailable = dto.IsAvailable,
            CurrentLatitude = dto.CurrentLatitude,
            CurrentLongitude = dto.CurrentLongitude
        };
    }

    /// <summary>
    /// Maps DriverInfoData to Driver entity
    /// </summary>
    /// <param name="driverInfo">The DriverInfoData containing driver information</param>
    /// <param name="applicationUserId">The ApplicationUser ID to associate with the driver</param>
    /// <returns>Driver entity object</returns>
    public static Driver ToDriver(this DriverInfoData driverInfo, Guid applicationUserId)
    {
        if (driverInfo == null)
            throw new ArgumentNullException(nameof(driverInfo));

        return new Driver
        {
            ApplicationUserId = applicationUserId,
            VehicleType = Enum.TryParse<VehicleType>(driverInfo.VehicleType, true, out var vt) ? vt : VehicleType.Cycle,
            IsAvailable = driverInfo.IsAvailable,
            Rating = 0,
            TotalDeliveries = 0,
            CurrentLatitude = driverInfo.CurrentLatitude,
            CurrentLongitude = driverInfo.CurrentLongitude
        };
    }
}