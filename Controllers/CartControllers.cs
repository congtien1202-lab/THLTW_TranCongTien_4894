using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using SellingWebsite.Data; // Gọi đúng tên nhà chứa DbContext
using SellingWebsite.Extensions;
using SellingWebsite.Models;

public class CartController : Controller
{
    private readonly ApplicationDbContext _context;
    // KHAI BÁO BIẾN Ở ĐÂY ĐỂ TRÁNH LỖI CS0103
    private readonly UserManager<ApplicationUser> _userManager;

    public CartController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    // Trang hiển thị giỏ hàng
    public IActionResult Index()
    {
        var cart = HttpContext.Session.GetJson<List<int>>("Cart") ?? new List<int>();

        var cartItems = cart.GroupBy(id => id)
            .Select(group => new CartItem
            {
                Product = _context.Products.FirstOrDefault(p => p.Id == group.Key),
                Quantity = group.Count()
            }).Where(x => x.Product != null).ToList();

        return View(cartItems);
    }

    // Thêm sản phẩm vào giỏ (Bạn nên có hàm này để nút ở trang Index chạy được)
    [HttpPost]
    public IActionResult AddToCart(int productId)
    {
        var cart = HttpContext.Session.GetJson<List<int>>("Cart") ?? new List<int>();
        cart.Add(productId);
        HttpContext.Session.SetJson("Cart", cart);
        return RedirectToAction("Index", "Product");
    }

    // Xóa sản phẩm khỏi giỏ
    public IActionResult RemoveFromCart(int productId)
    {
        var cart = HttpContext.Session.GetJson<List<int>>("Cart") ?? new List<int>();
        cart.RemoveAll(id => id == productId);
        HttpContext.Session.SetJson("Cart", cart);
        return RedirectToAction("Index");
    }

    [Authorize]
    [HttpPost]
    public async Task<IActionResult> Checkout()
    {
        var cartIds = HttpContext.Session.GetJson<List<int>>("Cart");
        if (cartIds == null || !cartIds.Any()) return RedirectToAction("Index");

        // Lấy ID của người dùng đang đăng nhập
        var userId = _userManager.GetUserId(User);

        var order = new Order
        {
            UserId = userId,
            OrderDate = DateTime.Now,
            Status = "Chờ xác nhận",
            OrderDetails = new List<OrderDetail>()
        };

        decimal total = 0;

        var groups = cartIds.GroupBy(id => id);
        foreach (var group in groups)
        {
            var product = _context.Products.Find(group.Key);
            if (product != null)
            {
                var detail = new OrderDetail
                {
                    ProductId = product.Id,
                    Quantity = group.Count(),
                    Price = product.Price
                };
                order.OrderDetails.Add(detail);
                total += detail.Price * detail.Quantity;
            }
        }

        order.TotalPrice = total;

        _context.Orders.Add(order);
        await _context.SaveChangesAsync();

        HttpContext.Session.Remove("Cart");

        // Gửi mã đơn hàng qua trang thành công để hiển thị
        return View("OrderSuccess", order.Id);
    }
}