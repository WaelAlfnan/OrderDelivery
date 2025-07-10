namespace OrderDelivery.Domain.Enums
{
    public enum OrderStatus
    {
        Pending,        // بانتظار سائق
        Assigned,       // تم التعيين لسائق
        PickedUp,       // السائق استلم الطلب من التاجر
        OnTheWay,       // في الطريق للتوصيل
        Delivered,      // تم التوصيل بنجاح
        Cancelled,      // تم الإلغاء
        Failed          // فشل التوصيل (مثلاً: العميل غير موجود)
    }
}