namespace OrderDelivery.Application.DataModels.RegistrationSteps
{
    public class DriverInfoData
    {
        public string VehicleType { get; set; } = string.Empty;
        public bool IsAvailable { get; set; }
        public decimal CurrentLatitude { get; set; }
        public decimal CurrentLongitude { get; set; }
    }
}
