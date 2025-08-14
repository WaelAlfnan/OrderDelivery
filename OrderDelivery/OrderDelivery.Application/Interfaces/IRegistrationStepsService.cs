using System.Threading.Tasks;
using OrderDelivery.Application.DTOs.RegistrationSteps;
using OrderDelivery.Domain.Entities;

namespace OrderDelivery.Application.Interfaces;

public interface IRegistrationStepsService
{
    Task<bool> StartRegistrationAsync(RegisterPhoneDto dto);
    Task<bool> VerifyPhoneAsync(VerifyPhoneDto dto);
    Task<bool> SetPasswordAsync(SetPasswordDto dto);
    Task<bool> SetRoleAsync(SetRoleDto dto);
    Task<bool> SetBasicInfoAsync(SetBasicInfoDto dto);
    Task<bool> SetMerchantInfoAsync(MerchantInfoDto dto);
    Task<bool> SetDriverInfoAsync(DriverInfoDto dto);
    Task<bool> SetVehicleInfoAsync(VehicleInfoDto dto);
    Task<bool> SetResidenceInfoAsync(ResidenceInfoDto dto);
    Task<PendingRegistration?> GetPendingRegistrationByPhoneAsync(string phoneNumber);
    Task<bool> CompleteRegistrationAsync(string phoneNumber);
} 