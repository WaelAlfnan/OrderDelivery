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
            PhoneNumber: "+1234567890",
            Province: "Riyadh",
            City: "Riyadh",
            District: "Al-Malaz",
            Street: "King Fahd Road",
            BuildingNumber: "123",
            Latitude: 24.7136m,
            Longitude: 46.6753m
        );

        // Act
        var result = dto.ToResidenceInfoData();

        // Assert
        Assert.NotNull(result);
        Assert.Equal("Riyadh", result.Province);
        Assert.Equal("Riyadh", result.City);
        Assert.Equal("Al-Malaz", result.District);
        Assert.Equal("King Fahd Road", result.Street);
        Assert.Equal("123", result.BuildingNumber);
        Assert.Equal(24.7136m, result.Latitude);
        Assert.Equal(46.6753m, result.Longitude);
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
    public void ToResidenceInfoData_DtoWithNullValues_ReturnsEmptyStrings()
    {
        // Arrange
        var dto = new ResidenceInfoDto(
            PhoneNumber: "+1234567890",
            Province: null!,
            City: null!,
            District: null!,
            Street: null!,
            BuildingNumber: null!,
            Latitude: 0m,
            Longitude: 0m
        );

        // Act
        var result = dto.ToResidenceInfoData();

        // Assert
        Assert.NotNull(result);
        Assert.Equal(string.Empty, result.Province);
        Assert.Equal(string.Empty, result.City);
        Assert.Equal(string.Empty, result.District);
        Assert.Equal(string.Empty, result.Street);
        Assert.Equal(string.Empty, result.BuildingNumber);
        Assert.Equal(0m, result.Latitude);
        Assert.Equal(0m, result.Longitude);
    }

    [Fact]
    public void ToResidence_ValidResidenceInfoData_ReturnsCorrectResidence()
    {
        // Arrange
        var residenceInfo = new ResidenceInfoData
        {
            Province = "Riyadh",
            City = "Riyadh",
            District = "Al-Malaz",
            Street = "King Fahd Road",
            BuildingNumber = "123",
            Latitude = 24.7136m,
            Longitude = 46.6753m
        };
        var driverId = Guid.NewGuid();

        // Act
        var result = residenceInfo.ToResidence(driverId);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(driverId, result.DriverId);
        Assert.Equal("Riyadh", result.Province);
        Assert.Equal("Riyadh", result.City);
        Assert.Equal("Al-Malaz", result.District);
        Assert.Equal("King Fahd Road", result.Street);
        Assert.Equal("123", result.BuildingNumber);
        Assert.Equal(24.7136m, result.Latitude);
        Assert.Equal(46.6753m, result.Longitude);
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
    public void ToResidence_ResidenceInfoDataWithEmptyValues_ReturnsCorrectResidence()
    {
        // Arrange
        var residenceInfo = new ResidenceInfoData
        {
            Province = string.Empty,
            City = string.Empty,
            District = string.Empty,
            Street = string.Empty,
            BuildingNumber = string.Empty,
            Latitude = 0m,
            Longitude = 0m
        };
        var driverId = Guid.NewGuid();

        // Act
        var result = residenceInfo.ToResidence(driverId);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(driverId, result.DriverId);
        Assert.Equal(string.Empty, result.Province);
        Assert.Equal(string.Empty, result.City);
        Assert.Equal(string.Empty, result.District);
        Assert.Equal(string.Empty, result.Street);
        Assert.Equal(string.Empty, result.BuildingNumber);
        Assert.Equal(0m, result.Latitude);
        Assert.Equal(0m, result.Longitude);
    }

    [Fact]
    public void ToResidence_ResidenceInfoDataWithSpecialCharacters_ReturnsCorrectResidence()
    {
        // Arrange
        var residenceInfo = new ResidenceInfoData
        {
            Province = "الرياض",
            City = "الرياض",
            District = "المليز",
            Street = "طريق الملك فهد",
            BuildingNumber = "123-أ",
            Latitude = 24.7136m,
            Longitude = 46.6753m
        };
        var driverId = Guid.NewGuid();

        // Act
        var result = residenceInfo.ToResidence(driverId);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(driverId, result.DriverId);
        Assert.Equal("الرياض", result.Province);
        Assert.Equal("الرياض", result.City);
        Assert.Equal("المليز", result.District);
        Assert.Equal("طريق الملك فهد", result.Street);
        Assert.Equal("123-أ", result.BuildingNumber);
        Assert.Equal(24.7136m, result.Latitude);
        Assert.Equal(46.6753m, result.Longitude);
    }
}
