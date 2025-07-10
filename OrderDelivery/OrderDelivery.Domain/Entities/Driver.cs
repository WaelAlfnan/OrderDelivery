using OrderDelivery.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderDelivery.Domain.Entities
{
    public class Driver
    {
        public Guid ApplicationUserId { get; set; }   // Foreign Key to User entity
        public bool IsAvailable { get; set; }
        public decimal Rating { get; set; }
        public int TotalDeliveries { get; set; }
        public decimal CurrentLatitude { get; set; }
        public decimal CurrentLongitude { get; set; }

        public virtual ApplicationUser AppUser { get; set; } = null!;
    }
}
