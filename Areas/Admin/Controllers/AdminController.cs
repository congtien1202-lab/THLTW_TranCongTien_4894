using Microsoft.AspNetCore.Mvc;

namespace SellingWebsite.Areas.Admin.Controllers
{
    [Area("Admin")] // Khai báo để hệ thống biết Controller này ở khu vực Admin
    public class AdminController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}