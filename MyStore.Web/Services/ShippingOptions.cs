using System;
namespace MyStore.Web.Services
{
    // Gia demo, doi phi tai mot cho neu can. Tinh phi tai server, KHONG tin gia gui tu form.
    public static class ShippingOptions
    {
        public const decimal EconomyFee = 15000m;
        public const decimal StandardFee = 30000m;
        public const decimal ExpressFee = 50000m;
        public const decimal StandardFreeFrom = 500000m;
        public static bool IsValid(string method)
        {
            return method == "Economy" || method == "Standard" || method == "Express";
        }
        public static decimal Fee(string method, decimal subtotal)
        {
            if (method == "Economy") return EconomyFee;
            if (method == "Standard") return subtotal >= StandardFreeFrom ? 0m : StandardFee;
            if (method == "Express") return ExpressFee;
            throw new InvalidOperationException("Vui lòng chọn hình thức giao hàng hợp lệ.");
        }
        public static string Name(string method)
        {
            switch (method)
            {
                case "Economy": return "Giao hàng tiết kiệm";
                case "Standard": return "Giao hàng tiêu chuẩn";
                case "Express": return "Giao hàng hỏa tốc";
                default: return "Chưa xác định";
            }
        }
    }
    public static class OrderLabels
    {
        public static string Status(string status)
        {
            switch(status)
            {
                case "Pending": return "Chờ thanh toán";
                case "Processing": return "Đang xử lý";
                case "Shipping": return "Đang vận chuyển";
                case "Delivered": return "Đã giao";
                case "Cancelled": return "Đã hủy";
                default: return status ?? "Chưa xác định";
            }
        }
        public static string Payment(string status)
        {
            switch(status)
            {
                case "Pending": return "Chưa thanh toán";
                case "Paid": return "Đã thanh toán";
                case "RefundPending": return "Chờ hoàn tiền";
                case "Cancelled": return "Đã hủy";
                default: return status ?? "Chưa xác định";
            }
        }
    }
}
