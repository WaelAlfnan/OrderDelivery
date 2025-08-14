using OrderDelivery.Application.DataModels.RegistrationSteps;
using OrderDelivery.Application.DTOs.RegistrationSteps;
using OrderDelivery.Domain.Entities;

namespace OrderDelivery.Application.Mappers.RegistrationStepsMappers;

public static class MerchantInfoMapper
{
    /// <summary>
    /// Maps MerchantInfoDto to MerchantInfoData
    /// </summary>
    /// <param name="dto">The DTO containing MerchantInfo</param>
    /// <returns>MerchantInfoData object</returns>
    public static MerchantInfoData ToMerchantInfoData(this MerchantInfoDto dto)
    {
        if (dto == null)
            throw new ArgumentNullException(nameof(dto));

        return new MerchantInfoData
        {
            StoreName = dto.StoreName,
            StoreType = dto.StoreType,
            StoreAddress = dto.StoreAddress,
            StoreLatitude = dto.StoreLatitude,
            StoreLongitude = dto.StoreLongitude,
            BusinessLicenseNumber = dto.BusinessLicenseNumber
        };
    }

    /// <summary>
    /// Maps MerchantInfoData to Merchant entity
    /// </summary>
    /// <param name="merchantInfo">The MerchantInfoData containing merchant information</param>
    /// <param name="applicationUserId">The ApplicationUser ID to associate with the merchant</param>
    /// <returns>Merchant entity object</returns>
    public static Merchant ToMerchant(this MerchantInfoData merchantInfo, Guid applicationUserId)
    {
        if (merchantInfo == null)
            throw new ArgumentNullException(nameof(merchantInfo));

        return new Merchant
        {
            ApplicationUserId = applicationUserId,
            StoreName = merchantInfo.StoreName,
            StoreType = merchantInfo.StoreType,
            StoreAddress = merchantInfo.StoreAddress,
            StoreLatitude = merchantInfo.StoreLatitude,
            StoreLongitude = merchantInfo.StoreLongitude,
            BusinessLicenseNumber = merchantInfo.BusinessLicenseNumber ?? string.Empty,
            Rating = 0,
            TotalOrders = 0
        };
    }
}