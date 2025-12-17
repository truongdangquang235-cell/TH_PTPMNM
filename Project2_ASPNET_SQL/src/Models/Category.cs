// Trong thư mục Models/Category.cs
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace GoCoffeeTea.Models
{
    [Table("Category")]
    public class Category
    {
        [Key]
        public int CategoryID { get; set; }

        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = string.Empty;

        [MaxLength(500)]
        public string Description { get; set; } = string.Empty;

        public bool IsActive { get; set; } = true;

        // Navigation Property: Một danh mục có nhiều sản phẩm
        public ICollection<Product> Products { get; set; } = new List<Product>();
    }
}