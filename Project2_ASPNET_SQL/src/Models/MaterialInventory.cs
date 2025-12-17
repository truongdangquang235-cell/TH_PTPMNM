// Trong thư mục Models/MaterialInventory.cs
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace GoCoffeeTea.Models
{
    [Table("MaterialInventory")]
    public class MaterialInventory
    {
        [Key]
        public int MaterialInventoryID { get; set; }

        [ForeignKey("Material")]
        public int MaterialID { get; set; }

        [Column(TypeName = "decimal(18, 2)")]
        public decimal Quantity { get; set; } = 0;

        public DateTime LastUpdated { get; set; } = DateTime.Now;

        // Navigation Property
        public Material Material { get; set; } = null!;
    }
}