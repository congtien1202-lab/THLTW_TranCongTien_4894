using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SellingWebsite.Data;
using System.Threading.Tasks;

namespace SellingWebsite.Areas.Admin.Controllers
{
    // BẮT BUỘC PHẢI CÓ THUỘC TÍNH NÀY ĐỂ HỆ THỐNG BIẾT NÓ THUỘC KHU VỰC ADMIN
    [Area("Admin")]
    public class CategoryController : Controller
    {
        private readonly ApplicationDbContext _context;

        public CategoryController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            // Lấy danh sách danh mục từ Database và trả về View
            var categories = await _context.Categories.ToListAsync();
            return View(categories);
        }
    }
}