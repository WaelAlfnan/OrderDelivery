namespace OrderDelivery.Application.DTOs.Responses;

using System;
 
public record AuthResponseDto(string AccessToken, string RefreshToken, DateTime ExpiresAt, UserDto User); 