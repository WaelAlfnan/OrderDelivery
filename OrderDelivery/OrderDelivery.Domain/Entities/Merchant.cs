using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderDelivery.Domain.Entities
{
    public class Merchant
    {
        public Guid ApplicationUserId { get; set; }     // Foreign Key to ApplicationUser entity

        public string StoreName { get; set; } = string.Empty;
        public string StoreAddress { get; set; } = string.Empty;
        public string StoreType { get; set; } = string.Empty; // (e.g., Supermarket, Restaurant, Pharmacy)
        public decimal StoreLatitude { get; set; } 
        public decimal StoreLongitude { get; set; }
        public decimal Rating { get; set; } // متوسط تقييم المتجر
        public int TotalOrders { get; set; } // إجمالي عدد الطلبات التي تم وضعها بواسطة هذا المتجر
        public string BusinessLicenseNumber { get; set; } = string.Empty; // رقم الترخيص التجاري


        // Navigation Property
        public virtual ApplicationUser AppUser { get; set; } = null!; // Required navigation property for the parent User
    }
}
