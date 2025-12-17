// Trong thư mục Controllers/CustomerController.cs

using GoCoffeeTea.Data;
using GoCoffeeTea.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GoCoffeeTea.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class CustomerController : Controller
    {
        private readonly ApplicationDbContext _context;

        public CustomerController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Customer
        // Hiển thị danh sách khách hàng
        public async Task<IActionResult> Index()
        {
            // Lấy tất cả khách hàng, sắp xếp theo tổng chi tiêu giảm dần (ví dụ)
            var customers = await _context.Customers
                                        .OrderByDescending(c => c.TotalSpent)
                                        .ToListAsync();
            return View(customers);
        }

        // GET: Customer/Details/5
        // Xem chi tiết khách hàng và lịch sử đơn hàng
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var customer = await _context.Customers
                .Include(c => c.Orders) // BẮT BUỘC: Load danh sách đơn hàng
                                        // Bạn có thể không cần ThenInclude ở đây vì Order Details không hiển thị
                                        // .ThenInclude(o => o.OrderDetails) 
                .FirstOrDefaultAsync(m => m.CustomerID == id);

            if (customer == null) return NotFound();

            return View(customer);
        }

        // GET: Customer/EditLoyalty/5
        // Form chỉnh sửa cấp độ thân thiết
        public async Task<IActionResult> EditLoyalty(int? id)
        {
            if (id == null) return NotFound();

            var customer = await _context.Customers.FindAsync(id);
            if (customer == null) return NotFound();

            return View(customer);
        }

        // POST: Customer/EditLoyalty/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditLoyalty(int id, [Bind("CustomerID, LoyaltyTier")] Customer customerUpdate)
        {
            if (id != customerUpdate.CustomerID) return NotFound();

            var customer = await _context.Customers.FindAsync(id);
            if (customer == null) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    // Chỉ cập nhật cấp độ thân thiết
                    customer.LoyaltyTier = customerUpdate.LoyaltyTier;
                    _context.Update(customer);
                    await _context.SaveChangesAsync();

                    TempData["SuccessMessage"] = $"Cập nhật cấp độ thân thiết của khách hàng '{customer.FullName}' thành công.";
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.Customers.Any(e => e.CustomerID == customer.CustomerID))
                        return NotFound();
                    else
                        throw;
                }
                return RedirectToAction(nameof(Index));
            }
            TempData["ErrorMessage"] = "Cập nhật thất bại. Vui lòng kiểm tra lại.";
            return View(customerUpdate);
        }
    }
}