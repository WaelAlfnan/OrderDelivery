using OrderDelivery.Domain.Enums;

namespace OrderDelivery.Domain.Entities
{
    public class Admin
    {
        public Guid Id { get; set; }
        public Guid ApplicationUserId { get; set; }

        public AdminRole Role { get; set; }
        public bool CanManageMerchants { get; set; }
        public bool CanManageDrivers { get; set; }
        public bool CanManageOrders { get; set; }
        public bool CanViewReports { get; set; }

        // Navigation Property
        public virtual ApplicationUser AppUser { get; set; } = null!;
    }
}
