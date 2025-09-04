namespace OrderDelivery.Application.DTOs.Requests
{
    /// <summary>
    /// Request DTO for refreshing JWT tokens
    /// </summary>
    public record RefreshTokenRequest(string RefreshToken);
}
