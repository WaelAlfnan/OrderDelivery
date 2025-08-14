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
            StoreName: "Test Store",
            StoreType: "Restaurant",
            StoreAddress: "123 Test Street",
            StoreLatitude: 25.123456m,
            StoreLongitude: 55.123456m,
            BusinessLicenseNumber: "LIC123456"
        );

        // Act
        var result = dto.ToMerchantInfoData();

        // Assert
        Assert.NotNull(result);
        Assert.Equal("Test Store", result.StoreName);
        Assert.Equal("Restaurant", result.StoreType);
        Assert.Equal("123 Test Street", result.StoreAddress);
        Assert.Equal(25.123456m, result.StoreLatitude);
        Assert.Equal(55.123456m, result.StoreLongitude);
        Assert.Equal("LIC123456", result.BusinessLicenseNumber);
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
    public void ToMerchant_ValidMerchantInfoData_ReturnsCorrectMerchant()
    {
        // Arrange
        var merchantInfo = new MerchantInfoData
        {
            StoreName = "Test Store",
            StoreType = "Restaurant",
            StoreAddress = "123 Test Street",
            StoreLatitude = 25.123456m,
            StoreLongitude = 55.123456m,
            BusinessLicenseNumber = "LIC123456"
        };
        var applicationUserId = Guid.NewGuid();

        // Act
        var result = merchantInfo.ToMerchant(applicationUserId);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(applicationUserId, result.ApplicationUserId);
        Assert.Equal("Test Store", result.StoreName);
        Assert.Equal("Restaurant", result.StoreType);
        Assert.Equal("123 Test Street", result.StoreAddress);
        Assert.Equal(25.123456m, result.StoreLatitude);
        Assert.Equal(55.123456m, result.StoreLongitude);
        Assert.Equal("LIC123456", result.BusinessLicenseNumber);
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
    public void ToMerchant_MerchantInfoDataWithNullBusinessLicenseNumber_ReturnsEmptyString()
    {
        // Arrange
        var merchantInfo = new MerchantInfoData
        {
            StoreName = "Test Store",
            StoreType = "Restaurant",
            StoreAddress = "123 Test Street",
            StoreLatitude = 25.123456m,
            StoreLongitude = 55.123456m,
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
    public void ToMerchant_DefaultValues_AreSetCorrectly()
    {
        // Arrange
        var merchantInfo = new MerchantInfoData
        {
            StoreName = "Test Store",
            StoreType = "Restaurant",
            StoreAddress = "123 Test Street",
            StoreLatitude = 25.123456m,
            StoreLongitude = 55.123456m,
            BusinessLicenseNumber = "LIC123456"
        };
        var applicationUserId = Guid.NewGuid();

        // Act
        var result = merchantInfo.ToMerchant(applicationUserId);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(0, result.Rating);
        Assert.Equal(0, result.TotalOrders);
    }
}
