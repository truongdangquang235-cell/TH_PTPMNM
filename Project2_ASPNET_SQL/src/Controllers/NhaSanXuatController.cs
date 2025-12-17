using Microsoft.AspNetCore.Mvc;
using WebApplication1.Models;
using Microsoft.EntityFrameworkCore; // Cần dùng thư viện này cho Find, Remove, Update

namespace WebApplication1.Controllers
{
    public class NhaSanXuatController : Controller
    {
        // Khởi tạo Context để làm việc với Database
        private readonly QLBHContext db = new QLBHContext();

        // GET: /NhaSanXuat/Index
        public IActionResult Index()
        {
            // Truyền danh sách Nhà sản xuất qua ViewBag để hiển thị trong View
            ViewBag.nsx = db.Nhasanxuats.ToList();
            return View();
        }

        // ---------------------------------------------------------------------
        // ## 1. Action Thêm Mới (Them)
        // ---------------------------------------------------------------------

        // GET: /NhaSanXuat/Them (Hiển thị form thêm mới)
        public IActionResult Them()
        {
            return View();
        }

        // POST: /NhaSanXuat/Them (Xử lý khi submit form)
        [HttpPost]
        [ValidateAntiForgeryToken] // Bảo mật chống tấn công CSRF
        public IActionResult Them(Nhasanxuat nsxMoi)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    // Thêm đối tượng mới vào DbSet
                    db.Nhasanxuats.Add(nsxMoi);
                    // Lưu thay đổi vào Database
                    db.SaveChanges();
                    // Chuyển hướng về trang Index sau khi thêm thành công
                    return RedirectToAction("Index");
                }
                catch (Exception ex)
                {
                    // Xử lý lỗi (ví dụ: in ra console hoặc ghi log)
                    ModelState.AddModelError("", "Lỗi khi thêm nhà sản xuất: " + ex.Message);
                }
            }
            // Nếu có lỗi validation hoặc lỗi try/catch, hiển thị lại form
            return View(nsxMoi);
        }

        // ---------------------------------------------------------------------
        // ## 2. Action Sửa (Sua)
        // ---------------------------------------------------------------------

        // GET: /NhaSanXuat/Sua/MaNSX (Hiển thị form với dữ liệu cũ)
        public IActionResult Sua(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return NotFound();
            }

            // Tìm Nhà sản xuất theo Mã (Khóa chính)
            var nsx = db.Nhasanxuats.Find(id);

            if (nsx == null)
            {
                return NotFound();
            }

            return View(nsx);
        }

        // POST: /NhaSanXuat/Sua (Xử lý khi submit form)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Sua(Nhasanxuat nsxDaSua)
        {
            // Lưu ý: Tên trường (ví dụ: Manhom) trong Model phải khớp với tên trường trong View.
            if (ModelState.IsValid)
            {
                try
                {
                    // Đánh dấu đối tượng là đã được sửa đổi
                    db.Nhasanxuats.Update(nsxDaSua);
                    // Hoặc: db.Entry(nsxDaSua).State = EntityState.Modified;

                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
                catch (DbUpdateConcurrencyException)
                {
                    // Xử lý lỗi đồng thời (nếu có)
                    ModelState.AddModelError("", "Lỗi đồng thời cơ sở dữ liệu.");
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", "Lỗi khi cập nhật nhà sản xuất: " + ex.Message);
                }
            }
            return View(nsxDaSua);
        }

        // ---------------------------------------------------------------------
        // ## 3. Action Xóa (Xoa)
        // ---------------------------------------------------------------------

        // GET: /NhaSanXuat/Xoa/MaNSX (Hiển thị màn hình xác nhận xóa)
        public IActionResult Xoa(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return NotFound();
            }

            var nsx = db.Nhasanxuats.Find(id);

            if (nsx == null)
            {
                return NotFound();
            }

            // Truyền đối tượng cần xóa sang View Xác nhận
            return View(nsx);
        }

        // POST: /NhaSanXuat/Xoa/MaNSX (Xử lý xóa)
        [HttpPost, ActionName("Xoa")] // ActionName để Action này được gọi khi form submit đến /Xoa
        [ValidateAntiForgeryToken]
        public IActionResult XacNhanXoa(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return NotFound();
            }

            var nsx = db.Nhasanxuats.Find(id);

            if (nsx == null)
            {
                // Đối tượng đã bị xóa bởi người dùng khác hoặc không tồn tại
                return RedirectToAction("Index");
            }

            // Xóa đối tượng khỏi DbSet
            db.Nhasanxuats.Remove(nsx);
            db.SaveChanges();

            return RedirectToAction("Index");
        }

        // Đảm bảo Dispose Context khi Controller kết thúc
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}