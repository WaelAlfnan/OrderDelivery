namespace OrderDelivery.Application.DataModels.RegistrationSteps
{
    public class VehicleInfoData
    {
        public string VehicleBrand { get; set; } = string.Empty;
        public string VehiclePlateNumber { get; set; } = string.Empty;
        public DateTime VehicleIssueDate { get; set; }
        public string ShasehPhotoUrl { get; set; } = string.Empty;
        public string EfragPhotoUrl { get; set; } = string.Empty;
    }
}
