// Trong thư mục Models/Material.cs
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace GoCoffeeTea.Models
{
    [Table("Material")]
    public class Material
    {
        [Key]
        public int MaterialID { get; set; }

        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = string.Empty;

        [Required]
        [MaxLength(20)]
        public string Unit { get; set; } = string.Empty;

        public bool IsActive { get; set; } = true;

        // Navigation Property
        public MaterialInventory Inventory { get; set; } = null!;
    }
}