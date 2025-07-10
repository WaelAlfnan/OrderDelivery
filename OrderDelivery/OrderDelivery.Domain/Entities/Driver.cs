
using OrderDelivery.Domain.Enums;

namespace OrderDelivery.Domain.Entities
{
    public class Driver
    {
        public Guid Id { get; set; }
        public Guid ApplicationUserId { get; set; }

        public VehicleType VehicleType { get; set; }
        public bool IsAvailable { get; set; }
        public decimal Rating { get; set; }
        public int TotalDeliveries { get; set; }
        public decimal CurrentLatitude { get; set; }
        public decimal CurrentLongitude { get; set; }



        // Navigation Properties
        public virtual ApplicationUser AppUser { get; set; } = null!;
        public virtual Vehicle Vehicle { get; set; }
        public virtual Residence Residence { get; set; }
    }
}
