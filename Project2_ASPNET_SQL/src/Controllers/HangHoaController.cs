using Microsoft.AspNetCore.Mvc;
using WebApplication1.Models;

namespace WebApplication1.Controllers
{
    public class HangHoaController : Controller
    {
        QLBHContext db = new QLBHContext();
        public IActionResult Index()
        {
            ViewBag.hh = db.Hanghoas;
            return View();
        }
    }
}
