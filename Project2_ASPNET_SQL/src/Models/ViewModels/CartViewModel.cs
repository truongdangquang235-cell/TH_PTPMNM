// Trong thư mục Models/ViewModels/CartViewModel.cs
using System.Collections.Generic;
using System.Linq;

namespace GoCoffeeTea.Models.ViewModels
{
    public class CartViewModel
    {
        public List<CartItemViewModel> Items { get; set; } = new List<CartItemViewModel>();

        // Tính tổng cộng của tất cả các mặt hàng
        public decimal GrandTotal => Items.Sum(i => i.SubTotal);
    }
}