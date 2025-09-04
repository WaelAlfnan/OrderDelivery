using OrderDelivery.Application.DataModels.RegistrationSteps;
using OrderDelivery.Application.DTOs.RegistrationSteps;
using OrderDelivery.Domain.Entities;

namespace OrderDelivery.Application.Mappers.RegistrationStepsMappers;

public static class ResidenceInfoMapper
{
    /// <summary>
    /// Maps ResidenceInfoDto to ResidenceInfoData
    /// </summary>
    /// <param name="dto">The DTO containing ResidenceInfo</param>
    /// <returns>ResidenceInfoData object</returns>
    public static ResidenceInfoData ToResidenceInfoData(this ResidenceInfoDto dto)
    {
        if (dto == null)
            throw new ArgumentNullException(nameof(dto));

        return new ResidenceInfoData
        {
            Province = dto.Province,
            City = dto.City,
            District = dto.District,
            Street = dto.Street,
            BuildingNumber = dto.BuildingNumber,
            Latitude = dto.Latitude,
            Longitude = dto.Longitude
        };
    }

    /// <summary>
    /// Maps ResidenceInfoData to Residence entity
    /// </summary>
    /// <param name="residenceInfo">The ResidenceInfoData containing residence information</param>
    /// <param name="driverId">The Driver ID to associate with the residence</param>
    /// <returns>Residence entity object</returns>
    public static Residence ToResidence(this ResidenceInfoData residenceInfo, Guid driverId)
    {
        if (residenceInfo == null)
            throw new ArgumentNullException(nameof(residenceInfo));

        return new Residence
        {
            DriverId = driverId,
            Province = residenceInfo.Province,
            City = residenceInfo.City,
            District = residenceInfo.District,
            Street = residenceInfo.Street,
            BuildingNumber = residenceInfo.BuildingNumber,
            Latitude = residenceInfo.Latitude,
            Longitude = residenceInfo.Longitude
        };
    }
}