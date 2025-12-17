// Trong Areas/Admin/Controllers/ReportController.cs

using GoCoffeeTea.Data;
using GoCoffeeTea.Models.ViewModels; // Cần thiết cho các ViewModel Báo cáo
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace GoCoffeeTea.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class ReportController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ReportController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Admin/Report/Index (Dashboard/Tổng quan)
        public async Task<IActionResult> Index()
        {
            // Lấy dữ liệu 30 ngày gần nhất
            DateTime startDate = DateTime.Today.AddDays(-30);

            // Truy vấn tổng hợp
            var totalSales = await _context.Orders
                .Where(o => o.OrderDate >= startDate && o.Status == "Completed")
                .SumAsync(o => o.FinalAmount);

            var totalOrders = await _context.Orders
                .Where(o => o.OrderDate >= startDate && o.Status == "Completed")
                .CountAsync();

            // Lấy sản phẩm bán chạy nhất
            var topProducts = await _context.OrderDetails
                .Include(od => od.Product)
                .Where(od => od.Order.OrderDate >= startDate && od.Order.Status == "Completed")
                .GroupBy(od => od.ProductID)
                .Select(g => new TopProductViewModel
                {
                    ProductName = g.First().Product.Name,
                    TotalQuantity = g.Sum(od => od.Quantity)
                })
                .OrderByDescending(t => t.TotalQuantity)
                .Take(5)
                .ToListAsync();

            // Khởi tạo Dashboard ViewModel
            var model = new DashboardViewModel
            {
                TotalSalesLast30Days = totalSales,
                TotalOrdersLast30Days = totalOrders,
                TopSellingProducts = topProducts,
                // Thêm các thống kê khác nếu cần (ví dụ: Tồn kho thấp)
            };

            return View(model);
        }

        // GET: Admin/Report/DailySales (Báo cáo Doanh thu chi tiết theo ngày)
        public async Task<IActionResult> DailySales()
        {
            // Nhóm dữ liệu đơn hàng theo ngày
            var dailySales = await _context.Orders
                .Where(o => o.Status == "Completed")
                .GroupBy(o => o.OrderDate.Date)
                .Select(g => new DailySalesViewModel
                {
                    Date = g.Key,
                    TotalAmount = g.Sum(o => o.FinalAmount),
                    OrderCount = g.Count()
                })
                .OrderByDescending(d => d.Date)
                .ToListAsync();

            return View(dailySales);
        }
    }
}