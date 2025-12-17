// Trong thư mục Controllers/DiscountController.cs

using GoCoffeeTea.Data;
using GoCoffeeTea.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GoCoffeeTea.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class DiscountController : Controller
    {
        private readonly ApplicationDbContext _context;

        public DiscountController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Discount
        public async Task<IActionResult> Index()
        {
            var discounts = await _context.Discounts.OrderByDescending(d => d.StartDate).ToListAsync();
            return View(discounts);
        }

        // GET: Discount/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Discount/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Code, Name, Type, Value, MinOrderAmount, StartDate, EndDate, IsActive")] Discount discount)
        {
            if (ModelState.IsValid)
            {
                _context.Add(discount);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = $"Thêm mã khuyến mãi '{discount.Code}' thành công.";
                return RedirectToAction(nameof(Index));
            }
            TempData["ErrorMessage"] = "Thêm khuyến mãi thất bại. Vui lòng kiểm tra lại thông tin.";
            return View(discount);
        }

        // GET: Discount/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var discount = await _context.Discounts.FindAsync(id);
            if (discount == null) return NotFound();

            return View(discount);
        }

        // POST: Discount/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("DiscountID, Code, Name, Type, Value, MinOrderAmount, StartDate, EndDate, IsActive")] Discount discount)
        {
            if (id != discount.DiscountID) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(discount);
                    await _context.SaveChangesAsync();
                    TempData["SuccessMessage"] = $"Cập nhật mã khuyến mãi '{discount.Code}' thành công.";
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.Discounts.Any(e => e.DiscountID == discount.DiscountID))
                        return NotFound();
                    else
                        throw;
                }
                return RedirectToAction(nameof(Index));
            }
            TempData["ErrorMessage"] = "Cập nhật thất bại. Vui lòng kiểm tra lại.";
            return View(discount);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Lock(int id)
        {
            var discount = await _context.Discounts.FindAsync(id);

            if (discount == null)
            {
                TempData["ErrorMessage"] = "Không tìm thấy mã khuyến mãi để khóa.";
                return NotFound();
            }

            if (!discount.IsActive)
            {
                TempData["ErrorMessage"] = $"Mã khuyến mãi {discount.Code} đã bị khóa rồi.";
            }
            else
            {
                discount.IsActive = false; // Đặt trạng thái hoạt động thành FALSE
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = $"Đã khóa mã khuyến mãi {discount.Code} thành công!";
            }

            return RedirectToAction(nameof(Index));
        }


        // POST: Admin/Discount/Unlock/5
        // Mở khóa mã khuyến mãi (IsActive = true)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Unlock(int id)
        {
            var discount = await _context.Discounts.FindAsync(id);

            if (discount == null)
            {
                TempData["ErrorMessage"] = "Không tìm thấy mã khuyến mãi để mở khóa.";
                return NotFound();
            }

            if (discount.IsActive)
            {
                TempData["ErrorMessage"] = $"Mã khuyến mãi {discount.Code} đã được kích hoạt rồi.";
            }
            else
            {
                discount.IsActive = true; // Đặt trạng thái hoạt động thành TRUE
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = $"Đã mở khóa mã khuyến mãi {discount.Code} thành công!";
            }

            return RedirectToAction(nameof(Index));
        }

    }
}