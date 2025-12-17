// Trong thư mục Controllers/ProductController.cs

using GoCoffeeTea.Data;
using GoCoffeeTea.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering; // Cần thiết cho SelectList
using Microsoft.EntityFrameworkCore;

namespace GoCoffeeTea.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class ProductController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ProductController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Product
        // Hiển thị danh sách sản phẩm
        public async Task<IActionResult> Index()
        {
            var products = await _context.Products
                .Include(p => p.Category)
                .OrderBy(p => p.CategoryID)
                .ToListAsync();
            return View(products);
        }

        // GET: Product/Create
        // Hiển thị form thêm mới
        public async Task<IActionResult> Create()
        {
            ViewData["CategoryID"] = new SelectList(
                await _context.Categories.Where(c => c.IsActive).ToListAsync(),
                "CategoryID",
                "Name"
            );
            ViewBag.Materials = await _context.Materials.ToListAsync();
            return View();
        }

        // POST: Product/Create
        // Xử lý lưu sản phẩm và công thức mới
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Product product, IFormCollection form)
        {
            // FIX MODELSTATE: Loại bỏ lỗi cho các Navigation Properties
            ModelState.Remove("Category");
            ModelState.Remove("Recipes");

            if (!ModelState.IsValid)
            {
                // Tải lại ViewData nếu Model không hợp lệ
                ViewData["CategoryID"] = new SelectList(await _context.Categories.Where(c => c.IsActive).ToListAsync(), "CategoryID", "Name", product.CategoryID);
                ViewBag.Materials = await _context.Materials.ToListAsync();
                TempData["ErrorMessage"] = "Vui lòng kiểm tra lại thông tin sản phẩm và công thức.";
                return View(product);
            }

            // 1. Lưu Sản phẩm trước để có ProductID
            _context.Add(product);
            await _context.SaveChangesAsync();

            // 2. Xử lý Công thức (Recipe) từ Form
            var materialIds = form["MaterialID"];
            var quantities = form["QuantityNeeded"];

            if (materialIds.Count > 0 && materialIds.Count == quantities.Count)
            {
                for (int i = 0; i < materialIds.Count; i++)
                {
                    // Lọc những dòng bị trống hoặc không hợp lệ (quantity <= 0)
                    if (int.TryParse(materialIds[i], out int materialId) &&
                        decimal.TryParse(quantities[i], out decimal quantityNeeded) &&
                        quantityNeeded > 0)
                    {
                        var recipe = new Recipe
                        {
                            ProductID = product.ProductID,
                            MaterialID = materialId,
                            QuantityNeeded = quantityNeeded
                        };
                        _context.Recipes.Add(recipe);
                    }
                }
                await _context.SaveChangesAsync(); // Lưu các Recipe mới
            }

            TempData["SuccessMessage"] = $"Thêm sản phẩm '{product.Name}' thành công!";
            return RedirectToAction(nameof(Index));
        }

        // -------------------------------------------------------------------

        // GET: Product/Edit/5
        // Tải sản phẩm và công thức để hiển thị form chỉnh sửa
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            // PHẢI INCLUDE Recipes để hiển thị công thức đã lưu
            var product = await _context.Products
                .Include(p => p.Recipes)
                .FirstOrDefaultAsync(m => m.ProductID == id);

            if (product == null)
            {
                return NotFound();
            }

            ViewData["CategoryID"] = new SelectList(
                await _context.Categories.Where(c => c.IsActive).ToListAsync(),
                "CategoryID",
                "Name",
                product.CategoryID
            );
            ViewBag.Materials = await _context.Materials.ToListAsync();

            return View(product);
        }

        // POST: Product/Edit/5
        // Xử lý cập nhật sản phẩm và đồng bộ hóa công thức
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ProductID, CategoryID, Name, Description, Price, IsActive")] Product product, IFormCollection form)
        {
            if (id != product.ProductID)
            {
                return NotFound();
            }

            // FIX LỖI MODELSTATE
            ModelState.Remove("Category");
            ModelState.Remove("Recipes");

            if (!ModelState.IsValid)
            {
                // Tải lại ViewData nếu Model không hợp lệ
                ViewData["CategoryID"] = new SelectList(await _context.Categories.Where(c => c.IsActive).ToListAsync(), "CategoryID", "Name", product.CategoryID);
                ViewBag.Materials = await _context.Materials.ToListAsync();
                TempData["ErrorMessage"] = "Vui lòng kiểm tra lại thông tin sản phẩm và công thức.";
                return View(product);
            }

            // --- BƯỚC 1: Tải và Cập nhật thông tin Sản phẩm ---
            var productToUpdate = await _context.Products
                .Include(p => p.Recipes)
                .FirstOrDefaultAsync(p => p.ProductID == id);

            if (productToUpdate == null) return NotFound();

            // Cập nhật các trường cơ bản
            productToUpdate.CategoryID = product.CategoryID;
            productToUpdate.Name = product.Name;
            productToUpdate.Description = product.Description;
            productToUpdate.Price = product.Price;
            productToUpdate.IsActive = product.IsActive;

            // --- BƯỚC 2: Đồng bộ hóa Công thức (Xóa tất cả cũ và Thêm lại tất cả mới) ---

            _context.Recipes.RemoveRange(productToUpdate.Recipes); // Xóa Recipe cũ

            var materialIds = form["MaterialID"];
            var quantities = form["QuantityNeeded"];

            if (materialIds.Count > 0 && materialIds.Count == quantities.Count)
            {
                for (int i = 0; i < materialIds.Count; i++)
                {
                    if (int.TryParse(materialIds[i], out int materialId) &&
                        decimal.TryParse(quantities[i], out decimal quantityNeeded) &&
                        quantityNeeded > 0)
                    {
                        var newRecipe = new Recipe
                        {
                            ProductID = product.ProductID,
                            MaterialID = materialId,
                            QuantityNeeded = quantityNeeded
                        };
                        _context.Recipes.Add(newRecipe);
                    }
                }
            }

            // --- BƯỚC 3: Lưu tất cả thay đổi ---
            await _context.SaveChangesAsync();
            TempData["SuccessMessage"] = $"Cập nhật sản phẩm '{product.Name}' và công thức thành công!";

            return RedirectToAction(nameof(Index));
        }

        // -------------------------------------------------------------------
        // SOFT DELETE VÀ REACTIVATE
        // -------------------------------------------------------------------

        // POST: Product/Delete/5 (Xóa mềm)
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product != null)
            {
                // Set IsActive = false để ẩn khỏi menu
                product.IsActive = false;
                _context.Update(product);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = $"Đã ẩn (xóa mềm) sản phẩm '{product.Name}' thành công!";
            }
            return RedirectToAction(nameof(Index));
        }

        // POST: Product/Reactivate/5 (Mở lại)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Reactivate(int id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product != null)
            {
                // Kích hoạt lại: Đặt IsActive = true
                product.IsActive = true;
                _context.Update(product);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = $"Đã MỞ LẠI sản phẩm '{product.Name}' thành công và hiển thị trên menu.";
            }
            else
            {
                TempData["ErrorMessage"] = "Không tìm thấy sản phẩm cần mở lại.";
            }
            return RedirectToAction(nameof(Index));
        }
    }
}