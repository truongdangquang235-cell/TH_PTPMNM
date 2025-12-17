// Trong thư mục Models/ViewModels/CheckoutViewModel.cs
using System.ComponentModel.DataAnnotations;

namespace GoCoffeeTea.Models.ViewModels
{
    public class CheckoutViewModel
    {
        // Giỏ hàng (dữ liệu này được Controller điền vào từ Session)
        public CartViewModel Cart { get; set; } = new CartViewModel();

        // ID Khách hàng thân thiết được chọn từ dropdown (Nullable)
        public int? CustomerId { get; set; }

        // Mã khuyến mãi được nhập vào form
        public string? DiscountCode { get; set; }

        // ID của nhân viên đứng máy (Controller tự gán từ Session/Auth)
        public int EmployeeId { get; set; }

        // Tổng số tiền khách trả (có thể thêm nếu bạn xử lý tiền mặt/tiền thối)
        public decimal PaymentAmount { get; set; }
    }
}