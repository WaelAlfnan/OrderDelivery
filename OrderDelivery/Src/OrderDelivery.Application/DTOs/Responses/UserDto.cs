namespace OrderDelivery.Application.DTOs.Responses;

public record UserDto(Guid Id, string FullName, string PhoneNumber, bool IsPhoneConfirmed, string Role);