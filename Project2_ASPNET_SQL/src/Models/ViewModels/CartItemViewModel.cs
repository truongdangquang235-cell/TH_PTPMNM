// Trong thư mục Models/ViewModels/CartItemViewModel.cs

using System.ComponentModel.DataAnnotations;
// System.Linq không cần thiết trong file này

namespace GoCoffeeTea.Models.ViewModels
{
    // Đại diện cho một món hàng trong Giỏ hàng
    public class CartItemViewModel
    {
        [Required] // Đảm bảo ProductId phải có giá trị
        public int ProductId { get; set; }

        // ProductName không thể là null
        public string ProductName { get; set; } = string.Empty;

        [Required] // Đảm bảo giá phải có
        public decimal UnitPrice { get; set; }

        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Số lượng phải lớn hơn 0.")]
        public int Quantity { get; set; }

        // Thuộc tính chỉ đọc: Tổng tiền cho dòng này
        public decimal SubTotal => UnitPrice * Quantity;
    }
}
// *** QUAN TRỌNG: XÓA các định nghĩa CartViewModel và CheckoutViewModel khỏi file này. ***