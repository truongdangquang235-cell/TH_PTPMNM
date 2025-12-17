// Trong thư mục Controllers/AccountController.cs

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using GoCoffeeTea.Data;
using GoCoffeeTea.Models;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Text;
using System.Security.Cryptography;
using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Authorization; // Cần thiết cho các Controller Admin

namespace GoCoffeeTea.Controllers
{
    // Controller này không cần [Authorize] để cho phép truy cập trang đăng nhập
    public class AccountController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AccountController(ApplicationDbContext context)
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
                    // Chuyển đổi sang chuỗi hex (lowercase)
                    builder.Append(bytes[i].ToString("x2"));
                }
                return builder.ToString();
            }
        }

        // GET: Account/Login
        [HttpGet]
        public IActionResult Login(string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }

        // POST: Account/Login
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(string username, string password, string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;

            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                ModelState.AddModelError("", "Vui lòng nhập Tên đăng nhập và Mật khẩu.");
                return View();
            }

            // 1. Tạo chuỗi Hash từ mật khẩu người dùng nhập
            var hashedPassword = HashPassword(password);

            // 2. Tìm người dùng trong DB (chỉ tìm theo Username và IsActive)
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Username == username && u.IsActive == true);

            // 3. KIỂM TRA ĐĂNG NHẬP (So sánh Hash)
            // Nếu không tìm thấy user HOẶC PasswordHash không khớp
            if (user == null || !user.PasswordHash.Equals(hashedPassword, StringComparison.Ordinal))
            {
                ModelState.AddModelError("", "Tên đăng nhập hoặc mật khẩu không chính xác.");
                return View();
            }

            // 4. Xây dựng đối tượng Claims (Xác thực thành công)
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.UserID.ToString()),
                new Claim(ClaimTypes.Name, user.FullName ?? user.Username),
                new Claim(ClaimTypes.Role, user.Role),
            };

            var claimsIdentity = new ClaimsIdentity(
                claims, CookieAuthenticationDefaults.AuthenticationScheme);

            var authProperties = new AuthenticationProperties
            {
                IsPersistent = true
            };

            // 5. Đăng nhập và tạo Cookie
            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(claimsIdentity),
                authProperties);

            // 6. Chuyển hướng
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }

            // Chuyển hướng mặc định sau khi đăng nhập thành công
            if (user.Role == "Admin")
            {
                // Admin chuyển hướng vào Dashboard Báo cáo
                return RedirectToAction("Index", "Report", new { area = "Admin" });
            }

            // Employee chuyển hướng vào màn hình POS
            return RedirectToAction("Index", "Sale");
        }

        // POST: Account/Logout
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            // Chuyển hướng về trang Login sau khi đăng xuất
            return RedirectToAction("Login", "Account");
        }
    }
}