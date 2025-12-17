// Trong Models/ViewModels/DailySalesViewModel.cs

using System;

namespace GoCoffeeTea.Models.ViewModels
{
    public class DailySalesViewModel
    {
        public DateTime Date { get; set; }
        public decimal TotalAmount { get; set; }
        public int OrderCount { get; set; }
    }
}