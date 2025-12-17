// Trong Models/ViewModels/DashboardViewModel.cs

using System.Collections.Generic;

namespace GoCoffeeTea.Models.ViewModels
{
    public class DashboardViewModel
    {
        public decimal TotalSalesLast30Days { get; set; }
        public int TotalOrdersLast30Days { get; set; }
        public List<TopProductViewModel> TopSellingProducts { get; set; } = new List<TopProductViewModel>();
    }

    public class TopProductViewModel
    {
        public string ProductName { get; set; } = string.Empty;
        public int TotalQuantity { get; set; }
    }
}