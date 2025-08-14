using OrderDelivery.Application.DataModels.RegistrationSteps;
using OrderDelivery.Application.DTOs.RegistrationSteps;
using OrderDelivery.Application.Mappers.RegistrationStepsMappers;
using OrderDelivery.Domain.Entities;
using OrderDelivery.Domain.Enums;
using Xunit;

namespace OrderDelivery.UnitTests.Mappers;

public class DriverInfoMapperTests
{
    [Fact]
    public void ToDriverInfoData_ValidDto_ReturnsCorrectDriverInfoData()
    {
        // Arrange
        var dto = new DriverInfoDto(
            VehicleType: "Motorcycle",
            IsAvailable: true,
            CurrentLatitude: 25.123456m,
            CurrentLongitude: 55.123456m
        );

        // Act
        var result = dto.ToDriverInfoData();

        // Assert
        Assert.NotNull(result);
        Assert.Equal("Motorcycle", result.VehicleType);
        Assert.True(result.IsAvailable);
        Assert.Equal(25.123456m, result.CurrentLatitude);
        Assert.Equal(55.123456m, result.CurrentLongitude);
    }

    [Fact]
    public void ToDriverInfoData_NullDto_ThrowsArgumentNullException()
    {
        // Arrange
        DriverInfoDto? dto = null;

        // Act & Assert
        var exception = Assert.Throws<ArgumentNullException>(() => dto!.ToDriverInfoData());
        Assert.Equal("dto", exception.ParamName);
    }

    [Fact]
    public void ToDriver_ValidDriverInfoData_ReturnsCorrectDriver()
    {
        // Arrange
        var driverInfo = new DriverInfoData
        {
            VehicleType = "Motorcycle",
            IsAvailable = true,
            CurrentLatitude = 25.123456m,
            CurrentLongitude = 55.123456m
        };
        var applicationUserId = Guid.NewGuid();

        // Act
        var result = driverInfo.ToDriver(applicationUserId);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(applicationUserId, result.ApplicationUserId);
        Assert.Equal(VehicleType.Motorcycle, result.VehicleType);
        Assert.True(result.IsAvailable);
        Assert.Equal(25.123456m, result.CurrentLatitude);
        Assert.Equal(55.123456m, result.CurrentLongitude);
        Assert.Equal(0, result.Rating);
        Assert.Equal(0, result.TotalDeliveries);
    }

    [Fact]
    public void ToDriver_NullDriverInfoData_ThrowsArgumentNullException()
    {
        // Arrange
        DriverInfoData? driverInfo = null;
        var applicationUserId = Guid.NewGuid();

        // Act & Assert
        var exception = Assert.Throws<ArgumentNullException>(() => driverInfo!.ToDriver(applicationUserId));
        Assert.Equal("driverInfo", exception.ParamName);
    }

    [Fact]
    public void ToDriver_InvalidVehicleType_DefaultsToCycle()
    {
        // Arrange
        var driverInfo = new DriverInfoData
        {
            VehicleType = "InvalidType",
            IsAvailable = true,
            CurrentLatitude = 25.123456m,
            CurrentLongitude = 55.123456m
        };
        var applicationUserId = Guid.NewGuid();

        // Act
        var result = driverInfo.ToDriver(applicationUserId);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(VehicleType.Cycle, result.VehicleType);
    }

    [Fact]
    public void ToDriver_DefaultValues_AreSetCorrectly()
    {
        // Arrange
        var driverInfo = new DriverInfoData
        {
            VehicleType = "Motorcycle",
            IsAvailable = true,
            CurrentLatitude = 25.123456m,
            CurrentLongitude = 55.123456m
        };
        var applicationUserId = Guid.NewGuid();

        // Act
        var result = driverInfo.ToDriver(applicationUserId);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(0, result.Rating);
        Assert.Equal(0, result.TotalDeliveries);
    }
}
