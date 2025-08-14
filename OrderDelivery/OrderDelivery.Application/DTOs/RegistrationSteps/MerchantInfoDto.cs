namespace OrderDelivery.Application.DTOs.RegistrationSteps;

public record MerchantInfoDto(
    string PhoneNumber,
    string StoreName,
    string StoreType,
    string StoreAddress,
    decimal StoreLatitude,
    decimal StoreLongitude,
    string? BusinessLicenseNumber
); 