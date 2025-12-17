// Trong thư mục Models/Product.cs
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace GoCoffeeTea.Models
{
    [Table("Product")]
    public class Product
    {
        [Key]
        public int ProductID { get; set; }

        [ForeignKey("Category")]
        public int CategoryID { get; set; }

        [Required, MaxLength(150)]
        public string Name { get; set; } = string.Empty;

        [MaxLength(500)]
        public string? Description { get; set; }

        [Column(TypeName = "decimal(18, 2)")]
        public decimal Price { get; set; }

        [Display(Name = "Ảnh sản phẩm")]
        [StringLength(255)]
      

        public bool IsActive { get; set; } = true;
        // Navigation Property: Một sản phẩm có nhiều công thức (Recipes)
        public ICollection<Recipe> Recipes { get; set; } = new List<Recipe>();

        // Navigation Property: Dùng để tham chiếu đến Category
        public Category Category { get; set; } = null!;

    }
}