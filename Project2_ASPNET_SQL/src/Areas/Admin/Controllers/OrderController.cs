// Trong thư mục Controllers/OrderController.cs

using GoCoffeeTea.Data;
using GoCoffeeTea.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace GoCoffeeTea.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")] // Chỉ Admin mới được truy cập
    public class OrderController : Controller
    {
        private readonly ApplicationDbContext _context;

        public OrderController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Admin/Order/Index (Xem Danh sách TẤT CẢ Đơn hàng)
        public async Task<IActionResult> Index()
        {
            // Tải danh sách đơn hàng, bao gồm các chi tiết liên quan
            var orders = await _context.Orders
                .Include(o => o.Customer)
                .Include(o => o.Employee)
                .Include(o => o.Discount)
                // KHÔNG LỌC: Tải tất cả các đơn hàng, bao gồm Completed, Pending, Cancelled
                .OrderByDescending(o => o.OrderDate)
                .ToListAsync();

            return View(orders);
        }

        // GET: Admin/Order/Details/5 (Xem chi tiết một đơn hàng)
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            // Tải Order và tất cả các chi tiết liên quan (Bắt buộc phải có ThenInclude)
            var order = await _context.Orders
                .Include(o => o.Customer)
                .Include(o => o.Employee)
                .Include(o => o.Discount)
                .Include(o => o.OrderDetails)
                    .ThenInclude(od => od.Product) // Tải thông tin Sản phẩm cho OrderDetail
                .FirstOrDefaultAsync(m => m.OrderID == id);

            if (order == null)
            {
                return NotFound();
            }

            return View(order);
        }

        // POST: Admin/Order/Cancel/5 (Thay đổi trạng thái sang Hủy bỏ)
        [HttpPost]
        [ActionName("Cancel")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CancelConfirmed(int id)
        {
            var order = await _context.Orders.FindAsync(id);
            if (order != null)
            {
                if (order.Status == "Completed")
                {
                    // Logic thực hiện HỦY ĐƠN HÀNG

                    // LƯU Ý: Nếu bạn có hệ thống hoàn tác tồn kho, nó sẽ được đặt ở đây.
                    // Ví dụ: await _saleService.RevertStock(id);

                    order.Status = "Cancelled";
                    _context.Update(order);
                    await _context.SaveChangesAsync();
                    TempData["SuccessMessage"] = $"Đã HỦY đơn hàng #{id} thành công. Tồn kho chưa được hoàn tác.";
                }
                else
                {
                    TempData["ErrorMessage"] = $"Đơn hàng #{id} đang ở trạng thái {order.Status} và không thể hủy.";
                }
            }
            return RedirectToAction(nameof(Index));
        }
    }
}