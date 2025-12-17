// Trong thư mục Areas/Admin/Controllers/UserController.cs

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using GoCoffeeTea.Data;
using GoCoffeeTea.Models;
using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Authorization; // Cần thiết cho [Authorize]

namespace GoCoffeeTea.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")] // Bảo vệ Controller này, chỉ cho phép Admin truy cập
    public class UserController : Controller
    {
        private readonly ApplicationDbContext _context;

        public UserController(ApplicationDbContext context)
        {
            _context = context;
        }

        // Helper để Hash mật khẩu (Sử dụng SHA256 không salt)
        private string HashPassword(string password)
        {
            using (SHA256 sha256Hash = SHA256.Create())
            {
                byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(password));
                StringBuilder builder = new StringBuilder();
                for (int i = 0; i < bytes.Length; i++)
                {
                    builder.Append(bytes[i].ToString("x2"));
                }
                return builder.ToString();
            }
        }

        // GET: Admin/User
        public async Task<IActionResult> Index()
        {
            var users = await _context.Users.ToListAsync();
            return View(users);
        }

        // GET: Admin/User/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Admin/User/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Username, FullName, Email, Role, IsActive")] User user, string password)
        {
            // FIX LỖI VALIDATION: Tắt validation cho các trường được xử lý thủ công
            ModelState.Remove("PasswordHash");

            // Tùy chọn: Nếu FullName hoặc Email bị lỗi Validation, hãy thêm dòng này:
            // ModelState.Remove("FullName"); 
            // ModelState.Remove("Email");

            // 1. Kiểm tra mật khẩu thô (password)
            if (string.IsNullOrEmpty(password))
            {
                ModelState.AddModelError("password", "Mật khẩu không được để trống.");
                return View(user);
            }

            if (ModelState.IsValid)
            {
                // 2. Kiểm tra Username đã tồn tại chưa
                if (await _context.Users.AnyAsync(u => u.Username == user.Username))
                {
                    ModelState.AddModelError("Username", "Tên đăng nhập này đã tồn tại.");
                    return View(user);
                }

                user.PasswordHash = HashPassword(password); // Hash mật khẩu trước khi lưu
                user.CreatedDate = DateTime.Now;

                _context.Add(user);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = $"Tạo tài khoản {user.Username} thành công!";
                return RedirectToAction(nameof(Index));
            }
            // Nếu ModelState.IsValid thất bại do lỗi khác
            TempData["ErrorMessage"] = "Tạo tài khoản thất bại. Vui lòng kiểm tra lại.";
            return View(user);
        }

        // GET: Admin/User/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var user = await _context.Users.FindAsync(id);
            if (user == null) return NotFound();

            return View(user);
        }

        // POST: Admin/User/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("UserID, Username, FullName, Email, Role, IsActive")] User user, string newPassword)
        {
            if (id != user.UserID) return NotFound();

            // FIX LỖI VALIDATION: Loại bỏ các thuộc tính không được gửi từ form
            ModelState.Remove("PasswordHash");

            if (ModelState.IsValid)
            {
                var userToUpdate = await _context.Users.AsNoTracking().FirstOrDefaultAsync(u => u.UserID == id);
                if (userToUpdate == null) return NotFound();

                // 1. Xử lý Mật khẩu MỚI
                if (!string.IsNullOrEmpty(newPassword))
                {
                    user.PasswordHash = HashPassword(newPassword);
                }
                else
                {
                    // Giữ lại mật khẩu cũ nếu không nhập mật khẩu mới
                    user.PasswordHash = userToUpdate.PasswordHash;
                }

                try
                {
                    // Đặt lại ngày tạo ban đầu
                    user.CreatedDate = userToUpdate.CreatedDate;

                    _context.Update(user);
                    await _context.SaveChangesAsync();
                    TempData["SuccessMessage"] = $"Cập nhật tài khoản {user.Username} thành công!";
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.Users.Any(e => e.UserID == user.UserID))
                        return NotFound();
                    else
                        throw;
                }
                return RedirectToAction(nameof(Index));
            }
            TempData["ErrorMessage"] = "Cập nhật thất bại. Vui lòng kiểm tra lại.";
            return View(user);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Lock(int id)
        {
            var user = await _context.Users.FindAsync(id);

            if (user == null)
            {
                TempData["ErrorMessage"] = "Không tìm thấy tài khoản để khóa.";
                return NotFound();
            }

            // Đảm bảo không khóa tài khoản đang đăng nhập (hoặc tài khoản Admin nếu cần)
            if (User.Identity!.Name == user.Username)
            {
                TempData["ErrorMessage"] = "Không thể tự khóa tài khoản của chính mình.";
                return RedirectToAction(nameof(Index));
            }

            if (!user.IsActive)
            {
                TempData["ErrorMessage"] = $"Tài khoản {user.Username} đã bị khóa rồi.";
            }
            else
            {
                user.IsActive = false; // Đặt trạng thái hoạt động thành FALSE
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = $"Đã khóa tài khoản {user.Username} thành công!";
            }

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Unlock(int id)
        {
            var user = await _context.Users.FindAsync(id);

            if (user == null)
            {
                TempData["ErrorMessage"] = "Không tìm thấy tài khoản để mở khóa.";
                return NotFound();
            }

            if (user.IsActive)
            {
                TempData["ErrorMessage"] = $"Tài khoản {user.Username} đã được kích hoạt rồi.";
            }
            else
            {
                user.IsActive = true; // Đặt trạng thái hoạt động thành TRUE
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = $"Đã mở khóa tài khoản {user.Username} thành công!";
            }

            return RedirectToAction(nameof(Index));
        }
    }
}