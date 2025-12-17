// Trong thư mục Services/SaleService.cs

using GoCoffeeTea.Data;
using GoCoffeeTea.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace GoCoffeeTea.Services
{
    // Triển khai interface ISaleService
    public class SaleService : ISaleService
    {
        private readonly ApplicationDbContext _context;

        public SaleService(ApplicationDbContext context)
        {
            _context = context;
        }

        // =======================================================
        // 1. DEDUCT MATERIALS LOGIC (TRỪ TỒN KHO) - Đã FIX LỖI TRACKING
        // =======================================================

        // Trong Services/SaleService.cs

        public async Task<bool> DeductMaterialsAsync(int productId, int quantity)
        {
            // 1. Tải công thức (Recipe) (OK)
            var recipeItems = await _context.Recipes
                .Where(r => r.ProductID == productId)
                .ToListAsync();

            if (!recipeItems.Any())
            {
                return true;
            }

            // KHÔNG SỬ DỤNG GIAO DỊCH BAO NGOÀI. SỬ DỤNG GIAO DỊCH TỰ ĐỘNG CỦA EF CHO MỖI LỆNH UPDATE.
            // Nếu bạn muốn giữ transaction, hãy đảm bảo bạn đã cấu hình đúng. 
            // Tạm thời, tôi sẽ loại bỏ transaction và dựa vào từng SaveChangesAsync để tìm ra lỗi.

            try
            {
                foreach (var recipe in recipeItems)
                {
                    decimal totalMaterialNeeded = recipe.QuantityNeeded * quantity;

                    // Tải tồn kho KHÔNG THEO DÕI (AsNoTracking)
                    var inventory = await _context.MaterialInventories
                                        .AsNoTracking() // <<< KHÔNG GẮN VỚI CONTEXT HIỆN TẠI
                                        .SingleOrDefaultAsync(i => i.MaterialID == recipe.MaterialID);

                    // Kiểm tra: Tồn tại và đủ tồn kho
                    if (inventory == null || inventory.Quantity < totalMaterialNeeded)
                    {
                        // Nếu không đủ tồn kho, chúng ta PHẢI CẦN ROLLBACK ORDER (xử lý trong SaleController)
                        // LƯU Ý: Vì transaction đã bị gỡ bỏ khỏi đây, lỗi sẽ được đẩy ra ngoài.
                        return false;
                    }

                    // 3. TẠO MỘT BẢN GHI MỚI ĐỂ UPDATE (Vì đối tượng không được theo dõi)
                    var inventoryUpdate = new MaterialInventory
                    {
                        MaterialInventoryID = inventory.MaterialInventoryID,
                        MaterialID = inventory.MaterialID,
                        Quantity = inventory.Quantity - totalMaterialNeeded,
                        LastUpdated = DateTime.Now
                        // Các trường khác phải được gán giá trị từ đối tượng cũ nếu cần
                    };

                    // 4. THỰC HIỆN UPDATE TRỰC TIẾP (Không cần transaction ở đây)
                    // LƯU Ý: Dùng Update và SaveChangesAsync để ghi đè.
                    _context.MaterialInventories.Update(inventoryUpdate);
                    await _context.SaveChangesAsync();
                }

                // Sau khi tất cả các vòng lặp hoàn thành thành công, trả về true
                return true;
            }
            catch (System.Exception ex)
            {
                // Ghi log chi tiết
                System.IO.File.WriteAllText("TransactionError.log",
                    $"Lỗi khi trừ tồn kho: {ex.Message} - Inner: {ex.InnerException?.Message}");

                // Trả về false để SaleController xử lý việc rollback Order chính.
                return false;
            }
        }

        // =======================================================
        // 2. LOYALTY LOGIC (CẬP NHẬT CẤP ĐỘ KHÁCH HÀNG)
        // =======================================================

        public async Task UpdateCustomerLoyaltyAsync(int customerId, decimal amountSpent)
        {
            var customer = await _context.Customers
                                 .FirstOrDefaultAsync(c => c.CustomerID == customerId);

            if (customer == null) return;

            // 1. Tích lũy tổng chi tiêu và số đơn hàng
            customer.TotalSpent += amountSpent;
            customer.TotalOrders += 1;

            // 2. LOGIC TỰ ĐỘNG NÂNG CẤP (Tier Upgrade Logic)
            string newTier = customer.LoyaltyTier;

            // Định nghĩa các ngưỡng nâng cấp
            if (customer.TotalSpent >= 5000000)
            {
                newTier = "Platinum";
            }
            else if (customer.TotalSpent >= 2000000)
            {
                newTier = "Gold";
            }
            else if (customer.TotalSpent >= 500000)
            {
                newTier = "Silver";
            }
            else
            {
                newTier = "Bronze";
            }

            // 3. Cập nhật cấp độ nếu có sự thay đổi
            if (newTier != customer.LoyaltyTier)
            {
                customer.LoyaltyTier = newTier;
            }

            _context.Customers.Update(customer);
            await _context.SaveChangesAsync();
        }
    }
}