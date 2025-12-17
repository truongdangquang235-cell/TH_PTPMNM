using Microsoft.EntityFrameworkCore;
using GoCoffeeTea.Data;
using GoCoffeeTea.Services;
using Microsoft.AspNetCore.Authentication.Cookies;
using System; // Cần thiết cho TimeSpan

var builder = WebApplication.CreateBuilder(args);

// -----------------------------------------------------------
// PHẦN 1: ĐĂNG KÝ DỊCH VỤ (SERVICES)
// -----------------------------------------------------------

// Lấy Connection String từ appsettings.json
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ??
                       throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

// Đăng ký Database Context với SQL Server
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));

// Cấu hình Session cho Giỏ hàng (BẮT BUỘC cho SaleController)
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

// Cấu hình Authentication (Xác thực Cookie)
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Account/Login";       // Trang đăng nhập
        options.LogoutPath = "/Account/Logout";
        options.AccessDeniedPath = "/Home/AccessDenied"; // Trang từ chối truy cập
    });

builder.Services.AddAuthorization(); // Kích hoạt Authorization

// Đăng ký MVC Controllers và Views
builder.Services.AddControllersWithViews();

// Đăng ký Service Layer cho Logic Nghiệp vụ (Trừ tồn kho)
builder.Services.AddScoped<ISaleService, SaleService>();


var app = builder.Build();

// -----------------------------------------------------------
// PHẦN 2: CẤU HÌNH HTTP REQUEST PIPELINE (MIDDLEWARE)
// -----------------------------------------------------------

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}
// app.UseHttpsRedirection(); // Tùy chọn

app.UseStaticFiles(); // Cho phép truy cập file tĩnh

app.UseRouting();

// PHẢI GỌI USE AUTHENTICATION/AUTHORIZATION SAU UseRouting
app.UseAuthentication();
app.UseAuthorization();

// Sử dụng Session (PHẢI nằm sau UseRouting và trước Endpoints)
app.UseSession();

// -----------------------------------------------------------
// PHẦN 3: CẤU HÌNH ROUTING (ĐỊNH TUYẾN)
// -----------------------------------------------------------

// Cấu hình Routing cho AREA ADMIN
app.MapAreaControllerRoute(
    name: "Admin",
    areaName: "Admin",
    pattern: "Admin/{controller=Report}/{action=Index}/{id?}"); // Admin mặc định vào Report

// Cấu hình Routing Mặc định (cho POS/SaleController)
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Sale}/{action=Index}/{id?}"); // Đặt Sale là trang mặc định

app.Run();