using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderDelivery.Domain.Entities
{
    public class Residence
    {
        public Guid Id { get; set; }

        public Guid DriverId { get; set; }

        public string Province { get; set; } = string.Empty;
        public string City { get; set; } = string.Empty;
        public string District { get; set; } = string.Empty;
        public string Street { get; set; } = string.Empty;
        public string? BuildingNumber { get; set; }
        public decimal Latitude { get; set; }
        public decimal Longitude { get; set; }

        // Navigation Properties
        public virtual Driver Driver { get; set; } = null!;

    }
}
