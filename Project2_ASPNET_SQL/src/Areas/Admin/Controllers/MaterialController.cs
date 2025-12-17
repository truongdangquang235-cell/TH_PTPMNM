// Trong thư mục Controllers/MaterialController.cs

using GoCoffeeTea.Data;
using GoCoffeeTea.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GoCoffeeTea.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class MaterialController : Controller
    {
        private readonly ApplicationDbContext _context;

        public MaterialController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Material
        // Hiển thị danh sách Nguyên vật liệu
        public async Task<IActionResult> Index()
        {
            var materials = await _context.Materials.ToListAsync();
            return View(materials);
        }

        // GET: Material/Create
        // Hiển thị form thêm mới
        public IActionResult Create()
        {
            return View();
        }

        // POST: Material/Create
        // Xử lý lưu Nguyên vật liệu mới
        [HttpPost]
        [ValidateAntiForgeryToken]
        // Bổ sung IsActive vào Bind, dù nó có giá trị mặc định trong Model
        public async Task<IActionResult> Create([Bind("Name, Unit, IsActive")] Material material)
        {
            // Cần phải loại bỏ các lỗi ModelState liên quan đến Navigation Property (nếu có)
            ModelState.Remove("Inventory");
            ModelState.Remove("Recipes");

            if (ModelState.IsValid)
            {
                // 1. Nếu IsActive không được gửi từ form, đảm bảo nó là true
                material.IsActive = true;

                _context.Add(material);
                await _context.SaveChangesAsync();

                // 2. Tạo một bản ghi tồn kho ban đầu (Quantity = 0)
                var inventory = new MaterialInventory
                {
                    MaterialID = material.MaterialID,
                    Quantity = 0,
                    LastUpdated = DateTime.Now
                };
                _context.MaterialInventories.Add(inventory);
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = $"Thêm nguyên vật liệu '{material.Name}' thành công và khởi tạo tồn kho.";
                return RedirectToAction(nameof(Index));
            }

            // Nếu Model không hợp lệ
            TempData["ErrorMessage"] = "Vui lòng kiểm tra lại thông tin nguyên vật liệu. Tên và Đơn vị tính là bắt buộc.";
            return View(material);
        }

        // GET: Material/Edit/5
        // Hiển thị form chỉnh sửa
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var material = await _context.Materials.FindAsync(id);
            if (material == null) return NotFound();

            return View(material);
        }

        // POST: Material/Edit/5
        // Xử lý cập nhật thông tin
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("MaterialID, Name, Unit, IsActive")] Material material)
        {
            if (id != material.MaterialID) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(material);
                    await _context.SaveChangesAsync();
                    TempData["SuccessMessage"] = $"Cập nhật nguyên vật liệu '{material.Name}' thành công.";
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.Materials.Any(e => e.MaterialID == material.MaterialID))
                        return NotFound();
                    else
                        throw;
                }
                return RedirectToAction(nameof(Index));
            }
            TempData["ErrorMessage"] = "Cập nhật thất bại. Vui lòng kiểm tra lại.";
            return View(material);
        }

        // POST: Material/Delete/5 (Soft Delete)
        // Dùng để ẩn/khóa nguyên vật liệu không còn sử dụng
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var material = await _context.Materials.FindAsync(id);
            if (material != null)
            {
                material.IsActive = false;
                _context.Update(material);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = $"Đã ẩn nguyên vật liệu '{material.Name}'.";
            }
            return RedirectToAction(nameof(Index));
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Reactivate(int id)
        {
            var material = await _context.Materials.FindAsync(id);
            if (material != null)
            {
                // Kích hoạt lại: Đặt IsActive = true
                material.IsActive = true;
                _context.Update(material);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = $"Đã MỞ LẠI nguyên vật liệu '{material.Name}' thành công.";
            }
            else
            {
                TempData["ErrorMessage"] = "Không tìm thấy nguyên vật liệu cần mở lại.";
            }
            return RedirectToAction(nameof(Index));
        }
    }
}