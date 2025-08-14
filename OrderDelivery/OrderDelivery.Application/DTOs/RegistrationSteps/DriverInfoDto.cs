namespace OrderDelivery.Application.DTOs.RegistrationSteps;

public record DriverInfoDto(
    string PhoneNumber,
    string VehicleType,
    bool IsAvailable,
    decimal CurrentLatitude,
    decimal CurrentLongitude
);