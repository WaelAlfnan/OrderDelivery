namespace OrderDelivery.Application.DTOs.Requests;

public record PasswordResetSessionDto(string PhoneNumber, string SessionToken);
