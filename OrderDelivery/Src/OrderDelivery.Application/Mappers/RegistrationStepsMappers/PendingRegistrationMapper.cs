using OrderDelivery.Application.DTOs.Responses;
using OrderDelivery.Domain.Entities;

namespace OrderDelivery.Application.Mappers.RegistrationStepsMappers;

/// <summary>
/// Mapper for converting PendingRegistration entity to response DTO
/// </summary>
public static class PendingRegistrationMapper
{
    /// <summary>
    /// Converts PendingRegistration entity to PendingRegistrationResponseDto
    /// </summary>
    /// <param name="pendingRegistration">The pending registration entity</param>
    /// <returns>PendingRegistrationResponseDto or null if input is null</returns>
    public static PendingRegistrationResponseDto? ToResponseDto(this PendingRegistration? pendingRegistration)
    {
        if (pendingRegistration == null)
            return null;

        return new PendingRegistrationResponseDto
        {
            PhoneNumber = pendingRegistration.PhoneNumber,
            IsPhoneVerified = pendingRegistration.IsPhoneVerified,
            Role = pendingRegistration.Role,
            Step = pendingRegistration.Step,
            CreatedAt = pendingRegistration.CreatedAt,
            UpdatedAt = pendingRegistration.UpdatedAt
        };
    }
}
