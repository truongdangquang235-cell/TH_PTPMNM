// Trong thư mục Controllers/SaleController.cs

using GoCoffeeTea.Data;
using GoCoffeeTea.Models;
using GoCoffeeTea.Models.ViewModels;
using GoCoffeeTea.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using System.Collections.Generic; // Cần thiết cho List/IEnumerable
using System.Linq;
using System.Security.Claims;
using System.Text.Json;

namespace GoCoffeeTea.Controllers
{
    [Authorize]
    public class SaleController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly ISaleService _saleService;

        public SaleController(ApplicationDbContext context, ISaleService saleService)
        {
            _context = context;
            _saleService = saleService;
        }

        // Helper để lấy Giỏ hàng từ Session
        private CartViewModel GetCartFromSession()
        {
            var cartJson = HttpContext.Session.GetString("Cart");
            if (string.IsNullOrEmpty(cartJson))
            {
                return new CartViewModel();
            }
            return JsonSerializer.Deserialize<CartViewModel>(cartJson) ?? new CartViewModel();
        }

        // Helper để lưu Giỏ hàng vào Session
        private void SaveCartToSession(CartViewModel cart)
        {
            HttpContext.Session.SetString("Cart", JsonSerializer.Serialize(cart));
        }

        //===================================================================
        // #1. XEM MENU VÀ QUẢN LÝ GIỎ HÀNG (Index, AddToCart, RemoveFromCart, UpdateCart)
        //===================================================================

        public async Task<IActionResult> Index()
        {
            // Tải Categories và Products (chỉ những món đang hoạt động)
            ViewBag.Categories = await _context.Categories.Where(c => c.IsActive).ToListAsync();
            var products = await _context.Products.Where(p => p.IsActive).ToListAsync();
            ViewBag.Cart = GetCartFromSession();
            return View(products);
        }

        // POST: Sale/AddToCart
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddToCart(int productId, int quantity = 1)
        {
            if (quantity <= 0) return RedirectToAction(nameof(Index));

            var product = await _context.Products
                                        .Where(p => p.ProductID == productId)
                                        .Select(p => new { p.Name, p.Price })
                                        .FirstOrDefaultAsync();

            if (product == null) return NotFound();

            var cart = GetCartFromSession();
            var existingItem = cart.Items.FirstOrDefault(i => i.ProductId == productId);

            if (existingItem != null)
            {
                existingItem.Quantity += quantity;
            }
            else
            {
                cart.Items.Add(new CartItemViewModel
                {
                    ProductId = productId,
                    ProductName = product.Name,
                    UnitPrice = product.Price,
                    Quantity = quantity
                });
            }

            SaveCartToSession(cart);
            return RedirectToAction(nameof(Index));
        }

        // POST: Sale/RemoveFromCart
        [HttpPost]
        public IActionResult RemoveFromCart(int productId)
        {
            var cart = GetCartFromSession();
            var itemToRemove = cart.Items.FirstOrDefault(i => i.ProductId == productId);
            if (itemToRemove != null)
            {
                cart.Items.Remove(itemToRemove);
                SaveCartToSession(cart);
            }
            return RedirectToAction(nameof(Index));
        }

        // POST: Sale/UpdateCart
        [HttpPost]
        public IActionResult UpdateCart(int productId, int newQuantity)
        {
            if (newQuantity <= 0) return RemoveFromCart(productId);

            var cart = GetCartFromSession();
            var itemToUpdate = cart.Items.FirstOrDefault(i => i.ProductId == productId);
            if (itemToUpdate != null)
            {
                itemToUpdate.Quantity = newQuantity;
                SaveCartToSession(cart);
            }
            return RedirectToAction(nameof(Index));
        }


        //===================================================================
        // #2. THANH TOÁN (CHECKOUT & PROCESS ORDER)
        //===================================================================

        [HttpGet]
        public async Task<IActionResult> Checkout()
        {
            var cart = GetCartFromSession();
            if (!cart.Items.Any())
            {
                TempData["ErrorMessage"] = "Giỏ hàng trống! Vui lòng thêm món trước khi thanh toán.";
                return RedirectToAction(nameof(Index));
            }

            // Lấy danh sách khách hàng thân thiết và khuyến mãi
            ViewBag.LoyaltyCustomers = await _context.Customers
                .Where(c => c.IsLoyalty)
                .Select(c => new { c.CustomerID, c.FullName, c.PhoneNumber })
                .ToListAsync();

            ViewBag.ActiveDiscounts = await _context.Discounts
                .Where(d => d.IsActive && d.EndDate > DateTime.Now)
                .ToListAsync();

            var model = new CheckoutViewModel { Cart = cart };
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ProcessOrder(CheckoutViewModel model)
        {
            // Lấy ID nhân viên đang đăng nhập
            model.EmployeeId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "2");

            var cart = GetCartFromSession();
            if (!cart.Items.Any())
            {
                TempData["ErrorMessage"] = "Giỏ hàng trống hoặc đã bị xóa.";
                return RedirectToAction(nameof(Index));
            }
            model.Cart = cart;
            if (model.CustomerId == null)
            {
                model.CustomerId = 6 ;
            }
            // --- BƯỚC 1: Xử lý Khuyến mãi ---
            decimal discountValue = 0;
            Discount? appliedDiscount = null;
            int? discountIdToUse = null; // Biến tạm để lưu ID cuối cùng

            if (!string.IsNullOrEmpty(model.DiscountCode))
            {
                appliedDiscount = await _context.Discounts
                    .FirstOrDefaultAsync(d => d.Code == model.DiscountCode && d.IsActive && d.EndDate > DateTime.Now);

                if (appliedDiscount != null)
                {
                    if (appliedDiscount.Type == "Percent")
                        discountValue = model.Cart.GrandTotal * (appliedDiscount.Value / 100);
                    else
                        discountValue = appliedDiscount.Value;

                    if (discountValue > model.Cart.GrandTotal) discountValue = model.Cart.GrandTotal;
                    // Lưu ID của khuyến mãi được áp dụng
                    discountIdToUse = appliedDiscount.DiscountID;
                }
            }
            if (discountIdToUse == null)
            {
                // Gán DiscountID = 4 (Khuyến mãi 0 đồng)
                discountIdToUse = 4;
                
            }

            // Bắt đầu giao dịch lớn (Order + Inventory + Loyalty Update)
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                // --- BƯỚC 2: Tạo đối tượng Order ---
                var order = new Order
                {
                    CustomerID = model.CustomerId,
                    EmployeeID = model.EmployeeId,
                    OrderDate = DateTime.Now,
                    TotalAmount = model.Cart.GrandTotal,
                    DiscountID = appliedDiscount?.DiscountID,
                    DiscountAmount = discountValue,
                    FinalAmount = model.Cart.GrandTotal - discountValue,
                    Status = "Completed"
                };

                _context.Orders.Add(order);
                await _context.SaveChangesAsync(); // Lưu để có OrderID

                // --- BƯỚC 3: Tạo OrderDetails & Trừ tồn kho ---
                foreach (var item in cart.Items)
                {
                    var detail = new OrderDetail
                    {
                        OrderID = order.OrderID,
                        ProductID = item.ProductId,
                        Quantity = item.Quantity,
                        UnitPrice = item.UnitPrice,
                        SubTotal = item.SubTotal
                    };
                    _context.OrderDetails.Add(detail);

                    // TRỪ VẬT LIỆU
                    bool deductionSuccess = await _saleService.DeductMaterialsAsync(item.ProductId, item.Quantity);

                    if (!deductionSuccess)
                    {
                        await transaction.RollbackAsync();
                        TempData["ErrorMessage"] = $"Lỗi: Không đủ nguyên vật liệu để chế biến món '{item.ProductName}'. Vui lòng kiểm tra tồn kho.";
                        return RedirectToAction(nameof(Checkout));
                    }
                }

                // --- BƯỚC 4: CẬP NHẬT CẤP ĐỘ KHÁCH HÀNG ---
                if (model.CustomerId.HasValue)
                {
                    await _saleService.UpdateCustomerLoyaltyAsync(model.CustomerId.Value, order.FinalAmount);
                }

                // --- BƯỚC 5: Hoàn tất giao dịch ---
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                // Xóa giỏ hàng khỏi Session
                HttpContext.Session.Remove("Cart");

                TempData["SuccessMessage"] = $"Thanh toán thành công! Mã đơn hàng: {order.OrderID}";
                return RedirectToAction("Receipt", new { id = order.OrderID });
            }
            catch (Exception ex)
            {
                // Xử lý lỗi hệ thống/database
                await transaction.RollbackAsync();
                TempData["ErrorMessage"] = "Lỗi hệ thống trong quá trình xử lý đơn hàng.";
                return RedirectToAction(nameof(Checkout));
            }
        }

        //GET: Sale/Receipt/5
        public async Task<IActionResult> Receipt(int id)
        {
            var order = await _context.Orders
          .Include(o => o.OrderDetails)
              .ThenInclude(od => od.Product) // CẦN THIẾT
          .Include(o => o.Customer)        // CẦN THIẾT
          .Include(o => o.Discount)        // CẦN THIẾT
          .FirstOrDefaultAsync(o => o.OrderID == id);

            if (order == null) return NotFound();
            return View("Receipt", order);
        }

        // GET/POST: Sale/AddLoyaltyCustomer (Giữ nguyên)
        [HttpGet]
        public IActionResult AddLoyaltyCustomer()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddLoyaltyCustomer([Bind("FullName, PhoneNumber, Email, Address")] Customer customer)
        {
            if (ModelState.IsValid)
            {
                customer.IsLoyalty = true;
                customer.Points = 0;
                customer.JoinDate = System.DateTime.Now; // Đảm bảo trường JoinDate được điền
                customer.LoyaltyTier = "Bronze"; // Đặt cấp độ ban đầu
                customer.TotalSpent = 0;
                customer.TotalOrders = 0;

                _context.Customers.Add(customer);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = $"Đã thêm khách hàng thân thiết {customer.FullName} thành công!";
                return RedirectToAction(nameof(Checkout));
            }
            return View(customer);
        }
    }
}