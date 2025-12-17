// Trong thư mục Models/Discount.cs
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace GoCoffeeTea.Models
{
    [Table("Discount")]
    public class Discount
    {
        [Key]
        public int DiscountID { get; set; }

        [Required]
        [MaxLength(50)]
        public string Code { get; set; } = string.Empty;

        [Required]
        [MaxLength(150)]
        public string Name { get; set; } = string.Empty;

        [Required]
        [MaxLength(20)]
        public string Type { get; set; } = string.Empty;

        [Column(TypeName = "decimal(18, 2)")]
        public decimal Value { get; set; }

        [Column(TypeName = "decimal(18, 2)")]
        public decimal MinOrderAmount { get; set; } = 0;

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        public bool IsActive { get; set; } = true;
    }
}