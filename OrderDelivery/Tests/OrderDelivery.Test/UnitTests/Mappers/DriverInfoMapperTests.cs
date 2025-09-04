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
            PhoneNumber: "+1234567890",
            VehicleType: "Motorcycle",
            IsAvailable: true,
            CurrentLatitude: 24.7136m,
            CurrentLongitude: 46.6753m
        );

        // Act
        var result = dto.ToDriverInfoData();

        // Assert
        Assert.NotNull(result);
        Assert.Equal("Motorcycle", result.VehicleType);
        Assert.True(result.IsAvailable);
        Assert.Equal(24.7136m, result.CurrentLatitude);
        Assert.Equal(46.6753m, result.CurrentLongitude);
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
    public void ToDriverInfoData_DtoWithNullValues_ReturnsEmptyStrings()
    {
        // Arrange
        var dto = new DriverInfoDto(
            PhoneNumber: "+1234567890",
            VehicleType: null!,
            IsAvailable: false,
            CurrentLatitude: 0m,
            CurrentLongitude: 0m
        );

        // Act
        var result = dto.ToDriverInfoData();

        // Assert
        Assert.NotNull(result);
        Assert.Equal(string.Empty, result.VehicleType);
        Assert.False(result.IsAvailable);
        Assert.Equal(0m, result.CurrentLatitude);
        Assert.Equal(0m, result.CurrentLongitude);
    }

    [Fact]
    public void ToDriver_ValidDriverInfoData_ReturnsCorrectDriver()
    {
        // Arrange
        var driverInfo = new DriverInfoData
        {
            VehicleType = "Motorcycle",
            IsAvailable = true,
            CurrentLatitude = 24.7136m,
            CurrentLongitude = 46.6753m
        };
        var applicationUserId = Guid.NewGuid();

        // Act
        var result = driverInfo.ToDriver(applicationUserId);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(applicationUserId, result.ApplicationUserId);
        Assert.Equal(VehicleType.Motorcycle, result.VehicleType);
        Assert.True(result.IsAvailable);
        Assert.Equal(0, result.Rating);
        Assert.Equal(0, result.TotalDeliveries);
        Assert.Equal(24.7136m, result.CurrentLatitude);
        Assert.Equal(46.6753m, result.CurrentLongitude);
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
    public void ToDriver_InvalidVehicleType_ReturnsDefaultVehicleType()
    {
        // Arrange
        var driverInfo = new DriverInfoData
        {
            VehicleType = "InvalidType",
            IsAvailable = true,
            CurrentLatitude = 24.7136m,
            CurrentLongitude = 46.6753m
        };
        var applicationUserId = Guid.NewGuid();

        // Act
        var result = driverInfo.ToDriver(applicationUserId);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(VehicleType.Cycle, result.VehicleType); // Default value
    }

    [Fact]
    public void ToDriver_ValidVehicleTypes_ReturnsCorrectVehicleType()
    {
        // Arrange
        var applicationUserId = Guid.NewGuid();
        var testCases = new[]
        {
            ("Motorcycle", VehicleType.Motorcycle),
            ("Car", VehicleType.Car),
            ("Cycle", VehicleType.Cycle),
            ("motorcycle", VehicleType.Motorcycle), // Case insensitive
            ("CAR", VehicleType.Car), // Case insensitive
        };

        foreach (var (vehicleTypeString, expectedVehicleType) in testCases)
        {
            var driverInfo = new DriverInfoData
            {
                VehicleType = vehicleTypeString,
                IsAvailable = true,
                CurrentLatitude = 24.7136m,
                CurrentLongitude = 46.6753m
            };

            // Act
            var result = driverInfo.ToDriver(applicationUserId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(expectedVehicleType, result.VehicleType);
        }
    }
}
