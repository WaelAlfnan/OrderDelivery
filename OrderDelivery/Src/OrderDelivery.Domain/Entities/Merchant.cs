using System.Text.Json.Serialization;

namespace OrderDelivery.Domain.Entities
{
    public class Merchant
    {
        public Guid Id { get; set; }
        public Guid ApplicationUserId { get; set; }

        public string StoreName { get; set; } = string.Empty;
        public string StoreAddress { get; set; } = string.Empty;
        public string StoreType { get; set; } = string.Empty;
        public decimal StoreLatitude { get; set; }
        public decimal StoreLongitude { get; set; }
        public decimal Rating { get; set; }
        public int TotalOrders { get; set; }
        public string BusinessLicenseNumber { get; set; } = string.Empty;



        // Navigation Properties
        [JsonIgnore]
        public virtual ApplicationUser AppUser { get; set; } = null!;
        [JsonIgnore]
        public virtual ICollection<Order> Orders { get; set; } = new List<Order>();
    }
}
