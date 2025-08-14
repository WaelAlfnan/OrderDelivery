namespace OrderDelivery.Application.DTOs.Requests;
 
public record ResetPasswordDto(string PhoneNumber, string VerificationCode, string NewPassword); 