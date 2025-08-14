using OrderDelivery.Application.DataModels.RegistrationSteps;
using OrderDelivery.Application.DTOs.RegistrationSteps;
using OrderDelivery.Application.Mappers.RegistrationStepsMappers;
using OrderDelivery.Domain.Entities;
using Xunit;

namespace OrderDelivery.UnitTests.Mappers;

public class ResidenceInfoMapperTests
{
    [Fact]
    public void ToResidenceInfoData_ValidDto_ReturnsCorrectResidenceInfoData()
    {
        // Arrange
        var dto = new ResidenceInfoDto(
            Province: "Dubai",
            City: "Dubai City",
            District: "Downtown",
            Street: "Sheikh Zayed Road",
            BuildingNumber: "123",
            Latitude: 25.123456m,
            Longitude: 55.123456m
        );

        // Act
        var result = dto.ToResidenceInfoData();

        // Assert
        Assert.NotNull(result);
        Assert.Equal("Dubai", result.Province);
        Assert.Equal("Dubai City", result.City);
        Assert.Equal("Downtown", result.District);
        Assert.Equal("Sheikh Zayed Road", result.Street);
        Assert.Equal("123", result.BuildingNumber);
        Assert.Equal(25.123456m, result.Latitude);
        Assert.Equal(55.123456m, result.Longitude);
    }

    [Fact]
    public void ToResidenceInfoData_NullDto_ThrowsArgumentNullException()
    {
        // Arrange
        ResidenceInfoDto? dto = null;

        // Act & Assert
        var exception = Assert.Throws<ArgumentNullException>(() => dto!.ToResidenceInfoData());
        Assert.Equal("dto", exception.ParamName);
    }

    [Fact]
    public void ToResidence_ValidResidenceInfoData_ReturnsCorrectResidence()
    {
        // Arrange
        var residenceInfo = new ResidenceInfoData
        {
            Province = "Dubai",
            City = "Dubai City",
            District = "Downtown",
            Street = "Sheikh Zayed Road",
            BuildingNumber = "123",
            Latitude = 25.123456m,
            Longitude = 55.123456m
        };
        var driverId = Guid.NewGuid();

        // Act
        var result = residenceInfo.ToResidence(driverId);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(driverId, result.DriverId);
        Assert.Equal("Dubai", result.Province);
        Assert.Equal("Dubai City", result.City);
        Assert.Equal("Downtown", result.District);
        Assert.Equal("Sheikh Zayed Road", result.Street);
        Assert.Equal("123", result.BuildingNumber);
        Assert.Equal(25.123456m, result.Latitude);
        Assert.Equal(55.123456m, result.Longitude);
    }

    [Fact]
    public void ToResidence_NullResidenceInfoData_ThrowsArgumentNullException()
    {
        // Arrange
        ResidenceInfoData? residenceInfo = null;
        var driverId = Guid.NewGuid();

        // Act & Assert
        var exception = Assert.Throws<ArgumentNullException>(() => residenceInfo!.ToResidence(driverId));
        Assert.Equal("residenceInfo", exception.ParamName);
    }

    [Fact]
    public void ToResidence_ResidenceInfoDataWithNullBuildingNumber_HandlesCorrectly()
    {
        // Arrange
        var residenceInfo = new ResidenceInfoData
        {
            Province = "Dubai",
            City = "Dubai City",
            District = "Downtown",
            Street = "Sheikh Zayed Road",
            BuildingNumber = null,
            Latitude = 25.123456m,
            Longitude = 55.123456m
        };
        var driverId = Guid.NewGuid();

        // Act
        var result = residenceInfo.ToResidence(driverId);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(driverId, result.DriverId);
        Assert.Equal("Dubai", result.Province);
        Assert.Equal("Dubai City", result.City);
        Assert.Equal("Downtown", result.District);
        Assert.Equal("Sheikh Zayed Road", result.Street);
        Assert.Null(result.BuildingNumber);
        Assert.Equal(25.123456m, result.Latitude);
        Assert.Equal(55.123456m, result.Longitude);
    }
}
