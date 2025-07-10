using OrderDelivery.Domain.Enums;

namespace OrderDelivery.Domain.Entities
{
    public class Vehicle
    {
        public Guid Id { get; set; }
        public Guid DriverId { get; set; }

        public string? VehicleBrand { get; set; }
        public string? ShasehPhotoUrl { get; set; }
        public string? EfragPhotoUrl { get; set; }
        public DateTime? VehicleIssueDate { get; set; }
        public string? VehiclePlateNumber { get; set; }

        // Navigation Properties
        public virtual Driver Driver { get; set; } = null!;
    }
}
