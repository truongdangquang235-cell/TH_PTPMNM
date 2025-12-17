// Trong thư mục Controllers/InventoryController.cs

using GoCoffeeTea.Data;
using GoCoffeeTea.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GoCoffeeTea.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class InventoryController : Controller
    {
        private readonly ApplicationDbContext _context;

        public InventoryController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Inventory (Xem Danh sách Tồn kho Nguyên vật liệu)
        public async Task<IActionResult> Index()
        {
            // Tải tất cả tồn kho, bao gồm thông tin chi tiết về Nguyên vật liệu (Material)
            var inventoryList = await _context.MaterialInventories
                .Include(i => i.Material)
                .OrderBy(i => i.Material.Name)
                .ToListAsync();

            return View(inventoryList);
        }

        // GET: Inventory/Edit/5 (Lấy thông tin tồn kho để chỉnh sửa)
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            // Tải bản ghi tồn kho và Material
            var inventory = await _context.MaterialInventories
                .Include(i => i.Material)
                .FirstOrDefaultAsync(i => i.MaterialInventoryID == id);

            if (inventory == null)
            {
                return NotFound();
            }

            return View(inventory);
        }

        // POST: Inventory/Edit/5 (Cập nhật số lượng tồn kho)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("MaterialInventoryID, MaterialID, Quantity")] MaterialInventory inventory)
        {
            if (id != inventory.MaterialInventoryID)
            {
                return NotFound();
            }

            // Lấy lại thông tin tồn kho hiện tại (đã bao gồm Material) để xử lý lỗi và thông báo
            var existingInventory = await _context.MaterialInventories
                .Include(i => i.Material)
                .AsNoTracking()
                .FirstOrDefaultAsync(i => i.MaterialInventoryID == id);

            if (existingInventory == null)
            {
                return NotFound();
            }

            // Gán lại Navigation Property Material cho đối tượng đang được chỉnh sửa
            // Cần thiết để hiển thị tên vật liệu nếu xảy ra lỗi ModelState
            inventory.Material = existingInventory.Material;

            // FIX LỖI MODELSTATE: Loại bỏ validation cho Navigation Property và trường ngày tháng
            ModelState.Remove("Material");
            ModelState.Remove("LastUpdated");

            // Kiểm tra thêm: Số lượng phải >= 0
            if (inventory.Quantity < 0)
            {
                ModelState.AddModelError("Quantity", "Số lượng Tồn kho không thể là số âm.");
            }

            if (ModelState.IsValid)
            {
                try
                {
                    // Cập nhật thời gian
                    inventory.LastUpdated = DateTime.Now;

                    // Thực hiện cập nhật
                    _context.Update(inventory);
                    await _context.SaveChangesAsync();

                    TempData["SuccessMessage"] = $"Cập nhật tồn kho cho vật liệu '{inventory.Material.Name}' thành công. Tồn mới: {inventory.Quantity:N2}";
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.MaterialInventories.Any(e => e.MaterialInventoryID == inventory.MaterialInventoryID))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }

            // Nếu Model không hợp lệ
            TempData["ErrorMessage"] = "Vui lòng kiểm tra lại Số lượng Tồn kho (phải là số và >= 0).";
            return View(inventory);
        }
    }
}