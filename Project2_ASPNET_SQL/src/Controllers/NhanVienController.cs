using Microsoft.AspNetCore.Mvc;
using WebApplication1.Models;

namespace WebApplication1.Controllers
{
	public class NhanVienController : Controller
	{
        QLBHContext db = new QLBHContext();
        public IActionResult Index()
		{
			ViewBag.nv= db.Nhanviens;
			return View();
		}
	}
}
