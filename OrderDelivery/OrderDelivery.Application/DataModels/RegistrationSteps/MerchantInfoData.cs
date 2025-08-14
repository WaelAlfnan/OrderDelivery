namespace OrderDelivery.Application.DataModels.RegistrationSteps
{
    public class MerchantInfoData
    {
        public string StoreName { get; set; } = string.Empty;
        public string StoreType { get; set; } = string.Empty;
        public string StoreAddress { get; set; } = string.Empty;
        public decimal StoreLatitude { get; set; }
        public decimal StoreLongitude { get; set; }
        public string? BusinessLicenseNumber { get; set; }
    }
}
