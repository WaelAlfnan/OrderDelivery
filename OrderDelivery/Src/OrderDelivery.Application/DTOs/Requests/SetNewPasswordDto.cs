namespace OrderDelivery.Application.DTOs.Requests;

public record SetNewPasswordDto(string PhoneNumber, string VerificationCode, string NewPassword, string ConfirmPassword, string SessionToken);
