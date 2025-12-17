// Trong file Models/Customer.cs

using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Collections.Generic;

namespace GoCoffeeTea.Models
{
    [Table("Customer")]
    public class Customer
    {
        [Key]
        public int CustomerID { get; set; }

        [Required, MaxLength(100)]
        public string FullName { get; set; }

        [MaxLength(15)]
        public string? PhoneNumber { get; set; } // Cho phép NULL

        [MaxLength(100)]
        public string? Email { get; set; } // Cho phép NULL

        // --- Các trường đã có ---
        public int Points { get; set; } = 0;
        public bool IsLoyalty { get; set; } = false;

        // --- CÁC TRƯỜNG MỚI BỔ SUNG (FIX LỖI CS1061) ---

        [MaxLength(255)]
        public string? Address { get; set; } // Khắc phục lỗi Address

        public DateTime JoinDate { get; set; } = DateTime.Now; // Khắc phục lỗi JoinDate

        [MaxLength(50)]
        // Ví dụ: Bronze, Silver, Gold, Platinum
        public string LoyaltyTier { get; set; } = "Bronze"; // Khắc phục lỗi LoyaltyTier

        [Column(TypeName = "decimal(18, 2)")]
        // Tổng số tiền khách hàng đã chi tiêu
        public decimal TotalSpent { get; set; } = 0; // Khắc phục lỗi TotalSpent

        // Tổng số đơn hàng khách đã đặt
        public int TotalOrders { get; set; } = 0; // Khắc phục lỗi TotalOrders

        // --- Navigation Properties ---
        public ICollection<Order> Orders { get; set; } = new List<Order>();
    }
}