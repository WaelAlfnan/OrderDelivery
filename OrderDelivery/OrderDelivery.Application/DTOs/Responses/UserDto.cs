namespace OrderDelivery.Application.DTOs.Responses;
 
public record UserDto(string Id, string FullName, string PhoneNumber, bool IsPhoneConfirmed, string Role); 