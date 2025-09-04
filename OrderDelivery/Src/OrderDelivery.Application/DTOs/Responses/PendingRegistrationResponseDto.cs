namespace OrderDelivery.Application.DTOs.Responses;

/// <summary>
/// Response DTO for pending registration information without exposing internal ID
/// </summary>
public record PendingRegistrationResponseDto
{
    public string PhoneNumber { get; init; } = string.Empty;
    public bool IsPhoneVerified { get; init; }
    public string? Role { get; init; }
    public int Step { get; init; }
    public DateTime CreatedAt { get; init; }
    public DateTime UpdatedAt { get; init; }
}
