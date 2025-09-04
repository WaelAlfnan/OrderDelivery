using OrderDelivery.Application.DataModels.RegistrationSteps;
using OrderDelivery.Application.DTOs.RegistrationSteps;
using OrderDelivery.Application.Mappers.RegistrationStepsMappers;
using OrderDelivery.Domain.Entities;
using Xunit;

namespace OrderDelivery.UnitTests.Mappers;

public class VehicleInfoMapperTests
{
    [Fact]
    public void ToVehicleInfoData_ValidDto_ReturnsCorrectVehicleInfoData()
    {
        // Arrange
        var dto = new VehicleInfoDto(
            PhoneNumber: "+1234567890",
            VehicleBrand: "Honda",
            VehiclePlateNumber: "ABC123",
            VehicleIssueDate: "2020-01-01",
            ShasehPhoto: null!,
            EfragPhoto: null!
        );

        // Act
        var result = dto.ToVehicleInfoData();

        // Assert
        Assert.NotNull(result);
        Assert.Equal("Honda", result.VehicleBrand);
        Assert.Equal("ABC123", result.VehiclePlateNumber);
        Assert.Equal("2020-01-01", result.VehicleIssueDate);
        Assert.Equal(string.Empty, result.ShasehPhotoUrl);
        Assert.Equal(string.Empty, result.EfragPhotoUrl);
    }

    [Fact]
    public void ToVehicleInfoData_NullDto_ThrowsArgumentNullException()
    {
        // Arrange
        VehicleInfoDto? dto = null;

        // Act & Assert
        var exception = Assert.Throws<ArgumentNullException>(() => dto!.ToVehicleInfoData());
        Assert.Equal("dto", exception.ParamName);
    }

    [Fact]
    public void ToVehicleInfoData_DtoWithNullValues_ReturnsEmptyStrings()
    {
        // Arrange
        var dto = new VehicleInfoDto(
            PhoneNumber: "+1234567890",
            VehicleBrand: null!,
            VehiclePlateNumber: null!,
            VehicleIssueDate: "2020-01-01",
            ShasehPhoto: null!,
            EfragPhoto: null!
        );

        // Act
        var result = dto.ToVehicleInfoData();

        // Assert
        Assert.NotNull(result);
        Assert.Equal(string.Empty, result.VehicleBrand);
        Assert.Equal(string.Empty, result.VehiclePlateNumber);
    }

    [Fact]
    public void ToVehicle_ValidVehicleInfoData_ReturnsCorrectVehicle()
    {
        // Arrange
        var vehicleInfo = new VehicleInfoData
        {
            VehicleBrand = "Honda",
            VehiclePlateNumber = "ABC123",
            VehicleIssueDate = "2020-01-01",
            ShasehPhotoUrl = "https://example.com/shaseh.jpg",
            EfragPhotoUrl = "https://example.com/efrag.jpg"
        };
        var driverId = Guid.NewGuid();

        // Act
        var result = vehicleInfo.ToVehicle(driverId);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(driverId, result.DriverId);
        Assert.Equal("Honda", result.VehicleBrand);
        Assert.Equal("ABC123", result.VehiclePlateNumber);
        Assert.Equal("2020-01-01", result.VehicleIssueDate);
        Assert.Equal("https://example.com/shaseh.jpg", result.ShasehPhotoUrl);
        Assert.Equal("https://example.com/efrag.jpg", result.EfragPhotoUrl);
    }

    [Fact]
    public void ToVehicle_NullVehicleInfoData_ThrowsArgumentNullException()
    {
        // Arrange
        VehicleInfoData? vehicleInfo = null;
        var driverId = Guid.NewGuid();

        // Act & Assert
        var exception = Assert.Throws<ArgumentNullException>(() => vehicleInfo!.ToVehicle(driverId));
        Assert.Equal("vehicleInfo", exception.ParamName);
    }

    [Fact]
    public void ToVehicleInfoDto_ValidVehicleInfoData_ReturnsCorrectDto()
    {
        // Arrange
        var vehicleInfo = new VehicleInfoData
        {
            VehicleBrand = "Honda",
            VehiclePlateNumber = "ABC123",
            VehicleIssueDate = "2020-01-01",
            ShasehPhotoUrl = "https://example.com/shaseh.jpg",
            EfragPhotoUrl = "https://example.com/efrag.jpg"
        };
        var phoneNumber = "+1234567890";

        // Act
        var result = vehicleInfo.ToVehicleInfoDto(phoneNumber);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(phoneNumber, result.PhoneNumber);
        Assert.Equal("Honda", result.VehicleBrand);
        Assert.Equal("ABC123", result.VehiclePlateNumber);
        Assert.Equal("2020-01-01", result.VehicleIssueDate);
        Assert.Null(result.ShasehPhoto);
        Assert.Null(result.EfragPhoto);
    }

    [Fact]
    public void ToVehicleInfoDto_NullVehicleInfoData_ThrowsArgumentNullException()
    {
        // Arrange
        VehicleInfoData? vehicleInfo = null;
        var phoneNumber = "+1234567890";

        // Act & Assert
        var exception = Assert.Throws<ArgumentNullException>(() => vehicleInfo!.ToVehicleInfoDto(phoneNumber));
        Assert.Equal("data", exception.ParamName);
    }
}
