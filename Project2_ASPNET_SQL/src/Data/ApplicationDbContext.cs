// Trong thư mục Data/ApplicationDbContext.cs

using Microsoft.EntityFrameworkCore;
using GoCoffeeTea.Models;

// ******* ĐẢM BẢO THÊM NAMESPACE NÀY *******
namespace GoCoffeeTea.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        // Khai báo các DbSet TƯƠNG TỰ CÁC LỖI CS8618 BẰNG CÁCH THÊM = null!;

        // 1. Phân quyền và thông tin cơ bản
        public DbSet<User> Users { get; set; } = null!; // SỬA
        public DbSet<Category> Categories { get; set; } = null!; // SỬA
        public DbSet<Product> Products { get; set; } = null!; // SỬA
        public DbSet<Customer> Customers { get; set; } = null!; // SỬA
        public DbSet<Discount> Discounts { get; set; } = null!; // SỬA

        // 2. Quản lý Đơn hàng
        public DbSet<Order> Orders { get; set; } = null!; // SỬA
        public DbSet<OrderDetail> OrderDetails { get; set; } = null!; // SỬA

        // 3. Quản lý Tồn kho và Công thức (Nguyên vật liệu)
        public DbSet<Material> Materials { get; set; } = null!; // SỬA
        public DbSet<MaterialInventory> MaterialInventories { get; set; } = null!; // SỬA
        public DbSet<Recipe> Recipes { get; set; } = null!; // SỬA
    }
}