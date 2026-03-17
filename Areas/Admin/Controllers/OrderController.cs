using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SellingWebsite.Data;
using SellingWebsite.Models;

namespace SellingWebsite.Areas.Admin.Controllers
{
    [Area("Admin")] // Rất quan trọng để định tuyến đúng Area
    // [Authorize(Roles = "Admin")] // Tạm thời ẩn đi để test, sau này có phân quyền thì mở ra
    public class OrderController : Controller
    {
        private readonly ApplicationDbContext _context;

        public OrderController(ApplicationDbContext context)
        {
            _context = context;
        }

        // Danh sách tất cả đơn hàng
        public async Task<IActionResult> Index()
        {
            var orders = await _context.Orders
                .OrderByDescending(o => o.OrderDate)
                .ToListAsync();
            return View(orders);
        }

        // Xem chi tiết một đơn hàng
        public async Task<IActionResult> Details(int id)
        {
            var order = await _context.Orders
                .Include(o => o.OrderDetails)
                .FirstOrDefaultAsync(o => o.Id == id);

            if (order == null) return NotFound();

            // Nạp thông tin sản phẩm (bộ ảnh) cho từng chi tiết đơn hàng
            foreach (var detail in order.OrderDetails)
            {
                detail.Product = await _context.Products.FindAsync(detail.ProductId);
            }

            return View(order);
        }

        // Cập nhật trạng thái đơn hàng
        [HttpPost]
        public async Task<IActionResult> UpdateStatus(int id, string status)
        {
            var order = await _context.Orders.FindAsync(id);
            if (order != null)
            {
                order.Status = status;
                await _context.SaveChangesAsync();
            }
            return RedirectToAction("Details", new { id = id });
        }
    }
}