using Microsoft.AspNetCore.Http;

namespace OrderDelivery.Application.DataModels.RegistrationSteps
{
    public class BasicInfoData
    {
        public string FullName { get; set; } = string.Empty;
        public string PersonalPhotoUrl { get; set; } = string.Empty;
        public string NationalIdFrontPhotoUrl { get; set; } = string.Empty;
        public string NationalIdBackPhotoUrl { get; set; } = string.Empty;
        public string NationalIdNumber { get; set; } = string.Empty;
    }
}
