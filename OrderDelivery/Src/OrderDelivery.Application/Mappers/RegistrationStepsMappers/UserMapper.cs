using OrderDelivery.Application.DTOs.Responses;
using OrderDelivery.Domain.Entities;

namespace OrderDelivery.Application.Mappers.RegistrationStepsMappers;

public static class UserDtoMapper
{
    public static UserDto ToUserDto(this ApplicationUser user, string role)
    {
        if (user is null)
            throw new ArgumentNullException(nameof(user));
        return new UserDto(
            Id: user.Id,
            FullName: $"{user.FirstName} {user.LastName}".Trim(),
            PhoneNumber: user.PhoneNumber ?? string.Empty,
            IsPhoneConfirmed: user.PhoneNumberConfirmed,
            Role: role
        );
    }
}