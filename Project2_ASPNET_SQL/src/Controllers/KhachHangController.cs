using Microsoft.AspNetCore.Mvc;
using WebApplication1.Models;

namespace WebApplication1.Controllers
{
    public class KhachHangController : Controller
    {
        QLBHContext db = new QLBHContext();
        public IActionResult Index()
        {
            ViewBag.kh = db.Khachhangs;
            return PartialView();
        }
    }
}
