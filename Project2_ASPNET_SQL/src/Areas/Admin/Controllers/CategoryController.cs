// Trong thư mục Controllers/CategoryController.cs

using GoCoffeeTea.Data;
using GoCoffeeTea.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GoCoffeeTea.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class CategoryController : Controller
    {
        private readonly ApplicationDbContext _context;

        public CategoryController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Category
        public async Task<IActionResult> Index()
        {
            // Hiển thị danh sách tất cả các danh mục
            var categories = await _context.Categories.ToListAsync();
            return View(categories);
        }

        // GET: Category/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Category/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Name, Description")] Category category)
        {
            if (ModelState.IsValid)
            {
                // Mặc định IsActive = true (đã được đặt trong Model)
                _context.Add(category);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Thêm danh mục thành công!";
                return RedirectToAction(nameof(Index));
            }
            return View(category);
        }

        // GET: Category/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var category = await _context.Categories.FindAsync(id);
            if (category == null)
            {
                return NotFound();
            }
            return View(category);
        }

        // POST: Category/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("CategoryID, Name, Description, IsActive")] Category category)
        {
            if (id != category.CategoryID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(category);
                    await _context.SaveChangesAsync();
                    TempData["SuccessMessage"] = "Cập nhật danh mục thành công!";
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.Categories.Any(e => e.CategoryID == category.CategoryID))
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
            return View(category);
        }

        // POST: Category/Delete/5 (Xóa Logic)
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var category = await _context.Categories.FindAsync(id);
            if (category != null)
            {
                // Thực hiện xóa mềm (Soft Delete)
                category.IsActive = false;
                _context.Update(category);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Đã ẩn (xóa mềm) danh mục thành công!";
            }
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Unlock(int id)
        {
            var category = await _context.Categories.FindAsync(id); // Giả định tên DBSet là Categories

            if (category == null)
            {
                TempData["ErrorMessage"] = "Không tìm thấy danh mục để mở khóa.";
                return NotFound();
            }

            if (category.IsActive) // Giả định thuộc tính là IsActive
            {
                TempData["ErrorMessage"] = $"Danh mục {category.CategoryID} đã được kích hoạt rồi.";
            }
            else
            {
                category.IsActive = true; // Đặt trạng thái hoạt động thành TRUE
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = $"Đã mở khóa danh mục {category.CategoryID} thành công!";
            }

            return RedirectToAction(nameof(Index));
        }

    }
}