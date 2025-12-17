// Trong thư mục Models/Recipe.cs
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace GoCoffeeTea.Models
{
    [Table("Recipe")]
    public class Recipe
    {
        [Key]
        public int RecipeID { get; set; }

        [ForeignKey("Product")]
        public int ProductID { get; set; }

        [ForeignKey("Material")]
        public int MaterialID { get; set; }

        [Column(TypeName = "decimal(18, 4)")] // Độ chính xác cao hơn cho công thức
        public decimal QuantityNeeded { get; set; }

        // Navigation Properties
        public Product Product { get; set; } = null!;
        public Material Material { get; set; } = null!;
    }
}