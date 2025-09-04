using OrderDelivery.Domain.Enums;
using System.Text.Json.Serialization;

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
        [JsonIgnore]
        public virtual ApplicationUser AppUser { get; set; } = null!;
        [JsonIgnore]
        public virtual Vehicle? Vehicle { get; set; }
        [JsonIgnore]
        public virtual Residence Residence { get; set; }
        [JsonIgnore]
        public virtual ICollection<Order> OrdersDelivered { get; set; } = new List<Order>();
    }
}
