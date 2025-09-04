namespace OrderDelivery.Application.DTOs.Requests;

public record ResendCodeDto(string PhoneNumber, string SessionToken);
