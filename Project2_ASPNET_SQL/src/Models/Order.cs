// Trong thư mục Models/Order.cs
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace GoCoffeeTea.Models
{
    [Table("Order")]
    public class Order
    {
        [Key]
        public int OrderID { get; set; }

        [ForeignKey("Customer")]
        public int? CustomerID { get; set; } // Dùng int? vì có thể NULL (khách vãng lai)

        [ForeignKey("Employee")]
        public int EmployeeID { get; set; }

        public DateTime OrderDate { get; set; } = DateTime.Now;

        [Column(TypeName = "decimal(18, 2)")]
        public decimal TotalAmount { get; set; }

        [ForeignKey("Discount")]
        public int? DiscountID { get; set; } // Có thể NULL

        [Column(TypeName = "decimal(18, 2)")]
        public decimal DiscountAmount { get; set; } = 0;

        [Column(TypeName = "decimal(18, 2)")]
        public decimal FinalAmount { get; set; }

        [Required]
        [MaxLength(50)]
        public string Status { get; set; } = string.Empty;

        // Navigation Properties
        public Customer Customer { get; set; } = null!;
        public User Employee { get; set; } = null!;
        public Discount Discount { get; set; } = null!;
        public ICollection<OrderDetail> OrderDetails { get; set; } = new List<OrderDetail>();
    }
}