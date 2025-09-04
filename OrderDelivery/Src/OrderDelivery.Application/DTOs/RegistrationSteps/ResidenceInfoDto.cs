namespace OrderDelivery.Application.DTOs.RegistrationSteps;

public record ResidenceInfoDto(
    string PhoneNumber,
    string Province,
    string City,
    string District,
    string Street,
    string BuildingNumber,
    decimal Latitude,
    decimal Longitude
); 