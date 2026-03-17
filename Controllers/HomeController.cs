using Microsoft.AspNetCore.Mvc;

namespace SellingWebsite.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            // Tự động đá sang trang Đăng nhập khi vừa mở web
            return LocalRedirect("/Identity/Account/Login");
        }
    }
}