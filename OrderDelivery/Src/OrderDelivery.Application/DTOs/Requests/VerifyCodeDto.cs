namespace OrderDelivery.Application.DTOs.Requests;

public record VerifyCodeDto(string PhoneNumber, string VerificationCode, string SessionToken);
