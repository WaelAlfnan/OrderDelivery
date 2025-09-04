using OrderDelivery.Application.DataModels.RegistrationSteps;
using OrderDelivery.Application.DTOs.RegistrationSteps;
using OrderDelivery.Application.Mappers.RegistrationStepsMappers;
using OrderDelivery.Domain.Entities;
using Xunit;

namespace OrderDelivery.UnitTests.Mappers;

public class MerchantInfoMapperTests
{
    [Fact]
    public void ToMerchantInfoData_ValidDto_ReturnsCorrectMerchantInfoData()
    {
        // Arrange
        var dto = new MerchantInfoDto(
            PhoneNumber: "+1234567890",
            StoreName: "Test Store",
            StoreType: "Restaurant",
            StoreAddress: "123 Main St",
            StoreLatitude: 24.7136m,
            StoreLongitude: 46.6753m,
            BusinessLicenseNumber: "BL123456"
        );

        // Act
        var result = dto.ToMerchantInfoData();

        // Assert
        Assert.NotNull(result);
        Assert.Equal("Test Store", result.StoreName);
        Assert.Equal("Restaurant", result.StoreType);
        Assert.Equal("123 Main St", result.StoreAddress);
        Assert.Equal(24.7136m, result.StoreLatitude);
        Assert.Equal(46.6753m, result.StoreLongitude);
        Assert.Equal("BL123456", result.BusinessLicenseNumber);
    }

    [Fact]
    public void ToMerchantInfoData_NullDto_ThrowsArgumentNullException()
    {
        // Arrange
        MerchantInfoDto? dto = null;

        // Act & Assert
        var exception = Assert.Throws<ArgumentNullException>(() => dto!.ToMerchantInfoData());
        Assert.Equal("dto", exception.ParamName);
    }

    [Fact]
    public void ToMerchantInfoData_DtoWithNullValues_ReturnsEmptyStrings()
    {
        // Arrange
        var dto = new MerchantInfoDto(
            PhoneNumber: "+1234567890",
            StoreName: null!,
            StoreType: null!,
            StoreAddress: null!,
            StoreLatitude: 0m,
            StoreLongitude: 0m,
            BusinessLicenseNumber: null
        );

        // Act
        var result = dto.ToMerchantInfoData();

        // Assert
        Assert.NotNull(result);
        Assert.Equal(string.Empty, result.StoreName);
        Assert.Equal(string.Empty, result.StoreType);
        Assert.Equal(string.Empty, result.StoreAddress);
        Assert.Equal(0m, result.StoreLatitude);
        Assert.Equal(0m, result.StoreLongitude);
        Assert.Equal(string.Empty, result.BusinessLicenseNumber);
    }

    [Fact]
    public void ToMerchant_ValidMerchantInfoData_ReturnsCorrectMerchant()
    {
        // Arrange
        var merchantInfo = new MerchantInfoData
        {
            StoreName = "Test Store",
            StoreType = "Restaurant",
            StoreAddress = "123 Main St",
            StoreLatitude = 24.7136m,
            StoreLongitude = 46.6753m,
            BusinessLicenseNumber = "BL123456"
        };
        var applicationUserId = Guid.NewGuid();

        // Act
        var result = merchantInfo.ToMerchant(applicationUserId);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(applicationUserId, result.ApplicationUserId);
        Assert.Equal("Test Store", result.StoreName);
        Assert.Equal("Restaurant", result.StoreType);
        Assert.Equal("123 Main St", result.StoreAddress);
        Assert.Equal(24.7136m, result.StoreLatitude);
        Assert.Equal(46.6753m, result.StoreLongitude);
        Assert.Equal("BL123456", result.BusinessLicenseNumber);
        Assert.Equal(0, result.Rating);
        Assert.Equal(0, result.TotalOrders);
    }

    [Fact]
    public void ToMerchant_NullMerchantInfoData_ThrowsArgumentNullException()
    {
        // Arrange
        MerchantInfoData? merchantInfo = null;
        var applicationUserId = Guid.NewGuid();

        // Act & Assert
        var exception = Assert.Throws<ArgumentNullException>(() => merchantInfo!.ToMerchant(applicationUserId));
        Assert.Equal("merchantInfo", exception.ParamName);
    }

    [Fact]
    public void ToMerchant_MerchantInfoDataWithNullBusinessLicense_ReturnsEmptyString()
    {
        // Arrange
        var merchantInfo = new MerchantInfoData
        {
            StoreName = "Test Store",
            StoreType = "Restaurant",
            StoreAddress = "123 Main St",
            StoreLatitude = 24.7136m,
            StoreLongitude = 46.6753m,
            BusinessLicenseNumber = null
        };
        var applicationUserId = Guid.NewGuid();

        // Act
        var result = merchantInfo.ToMerchant(applicationUserId);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(string.Empty, result.BusinessLicenseNumber);
    }

    [Fact]
    public void ToMerchant_MerchantInfoDataWithEmptyValues_ReturnsCorrectMerchant()
    {
        // Arrange
        var merchantInfo = new MerchantInfoData
        {
            StoreName = string.Empty,
            StoreType = string.Empty,
            StoreAddress = string.Empty,
            StoreLatitude = 0m,
            StoreLongitude = 0m,
            BusinessLicenseNumber = string.Empty
        };
        var applicationUserId = Guid.NewGuid();

        // Act
        var result = merchantInfo.ToMerchant(applicationUserId);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(applicationUserId, result.ApplicationUserId);
        Assert.Equal(string.Empty, result.StoreName);
        Assert.Equal(string.Empty, result.StoreType);
        Assert.Equal(string.Empty, result.StoreAddress);
        Assert.Equal(0m, result.StoreLatitude);
        Assert.Equal(0m, result.StoreLongitude);
        Assert.Equal(string.Empty, result.BusinessLicenseNumber);
        Assert.Equal(0, result.Rating);
        Assert.Equal(0, result.TotalOrders);
    }
}
