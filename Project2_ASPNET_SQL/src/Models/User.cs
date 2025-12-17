// In your Models/User.cs file

using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GoCoffeeTea.Models
{
    [Table("User")]
    public class User
    {
        [Key]
        public int UserID { get; set; }

        [Required, MaxLength(50)]
        public string Username { get; set; } = null!;

        [Required, MaxLength(255)]
        public string PasswordHash { get; set; } = null!;

        [MaxLength(100)]
        public string? FullName { get; set; }

        // <<< ADD THIS MISSING PROPERTY TO FIX CS1061 >>>
        [MaxLength(100)]
        public string? Email { get; set; } // Allow NULL value if it's optional in the DB

        [Required, MaxLength(20)]
        public string Role { get; set; } = null!;

        public bool IsActive { get; set; } = true;

        public DateTime CreatedDate { get; set; } = DateTime.Now;

        // Navigation properties (if needed elsewhere)
    }
}