// Trong thư mục Models/OrderDetail.cs
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace GoCoffeeTea.Models
{
    [Table("OrderDetail")]
    public class OrderDetail
    {
        [Key]
        public int OrderDetailID { get; set; }

        [ForeignKey("Order")]
        public int OrderID { get; set; }

        [ForeignKey("Product")]
        public int ProductID { get; set; }

        public int Quantity { get; set; }

        [Column(TypeName = "decimal(18, 2)")]
        public decimal UnitPrice { get; set; }

        [Column(TypeName = "decimal(18, 2)")]
        public decimal SubTotal { get; set; }

        // Navigation Properties
        public Order Order { get; set; } = null!;
        public Product Product { get; set; } = null!;
    }
}