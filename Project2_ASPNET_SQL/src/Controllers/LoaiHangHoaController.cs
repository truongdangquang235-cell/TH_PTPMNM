using Microsoft.AspNetCore.Mvc;
using WebApplication1.Models;

namespace WebApplication1.Controllers
{
    public class LoaiHangHoaController : Controller
    {
        QLBHContext db = new QLBHContext();
        public IActionResult Index()
        {
            ViewBag.lh = db.Loaihanghoas;
            return View();
        }
    }
}
