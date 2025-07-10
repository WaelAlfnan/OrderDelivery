using OrderDelivery.Domain.Enums;

namespace OrderDelivery.Domain.Entities
{
    public class Order
    {
        public Guid Id { get; set; }

        public Guid MerchantId { get; set; }
        public Guid? DriverId { get; set; }

        public string CustomerName { get; set; } = string.Empty;
        public string CustomerPhoneNumber { get; set; } = string.Empty;
        public string DeliveryAddress { get; set; } = string.Empty;
        public decimal DeliveryLatitude { get; set; }
        public decimal DeliveryLongitude { get; set; }
        public decimal OrderValue { get; set; }
        public decimal DeliveryFee { get; set; }
        public PaymentMethod PaymentMethod { get; set; }
        public OrderStatus Status { get; set; }
        public string? SpecialInstructions { get; set; }

        public DateTime PlacedAt { get; set; } = DateTime.UtcNow; // تاريخ ووقت وضع الطلب
        public DateTime? AssignedAt { get; set; } // تاريخ ووقت تعيين السائق للطلب
        public DateTime? PickedUpAt { get; set; } // تاريخ ووقت استلام السائق للطلب من التاجر
        public DateTime? DeliveredAt { get; set; } // تاريخ ووقت تسليم الطلب للعميل
        public DateTime? CancelledAt { get; set; } // تاريخ ووقت إلغاء الطلب (إذا تم إلغاؤه)

        // Navigation Properties
        public virtual Merchant Merchant { get; set; } = null!;
        public virtual Driver? Driver { get; set; }
    }
}