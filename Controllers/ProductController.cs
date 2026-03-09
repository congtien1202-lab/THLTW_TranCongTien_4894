using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using WebsiteBanHang.Models;
using WebsiteBanHang.Repositories;
using WebsiteBanHang.Models;
using WebsiteBanHang.Repositories; 
public class ProductController : Controller
{
    private readonly IProductRepository _productRepository;
    private readonly ICategoryRepository _categoryRepository;
    public ProductController(IProductRepository productRepository,
    ICategoryRepository categoryRepository)
    {
        _productRepository = productRepository;
        _categoryRepository = categoryRepository;
    }
    [HttpGet]
    public IActionResult Add()
    {
        var categories = _categoryRepository.GetAllCategories();
        ViewBag.Categories = new SelectList(categories, "Id", "Name");
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Add(Product product, IFormFile imageUrl, List<IFormFile> imageUrls)
    {
        if (ModelState.IsValid)
        {
            // 1. Xử lý lưu ảnh đại diện
            if (imageUrl != null)
            {
                product.ImageUrl = await SaveImage(imageUrl);
            }

            // 2. Xử lý lưu danh sách ảnh phụ
            if (imageUrls != null && imageUrls.Count > 0)
            {
                product.ImageUrls = new List<string>();
                foreach (var file in imageUrls)
                {
                    product.ImageUrls.Add(await SaveImage(file));
                }
            }

            // 3. Lưu vào Repository
            _productRepository.Add(product);
            return RedirectToAction("Index");
        }

        // --- NẾU DỮ LIỆU LỖI (ModelState.IsValid == false) ---
        // Bạn phải load lại Categories ở đây để View không bị lỗi trắng Dropdown
        var categories = _categoryRepository.GetAllCategories();
        ViewBag.Categories = new SelectList(categories, "Id", "Name", product.CategoryId);

        return View(product);
    }
    // Các actions khác như Display, Update, Delete
    // Display a list of products
    public IActionResult Index()
    {
        var products = _productRepository.GetAll();
        return View(products);
    }
    // Display a single product
    public IActionResult Display(int id)
    {
        var product = _productRepository.GetById(id);
        if (product == null)
        {
            return NotFound();
        }
        return View(product);
    }
    // Show the product update form
    public IActionResult Update(int id)
    {
        var product = _productRepository.GetById(id);
        if (product == null)
        {
            return NotFound();
        }
        return View(product);
    }
    // Process the product update
    [HttpPost]
    public IActionResult Update(Product product)
    {
        if (ModelState.IsValid)
        {
            _productRepository.Update(product);
            return RedirectToAction("Index");
        }
        return View(product);
    }
    // Show the product delete confirmation
    public IActionResult Delete(int id)
    {
        var product = _productRepository.GetById(id);
        if (product == null)
        {
            return NotFound();
        }
        return View(product);
    }
    // Process the product deletion
    [HttpPost, ActionName("DeleteConfirmed")]
    public IActionResult DeleteConfirmed(int id)
    {
        _productRepository.Delete(id);
        return RedirectToAction("Index");
    }

    private async Task<string> SaveImage(IFormFile image)
    {
        // Thay đổi đường dẫn theo cấu hình của bạn
        var savePath = Path.Combine("wwwroot/images", image.FileName);
        using (var fileStream = new FileStream(savePath, FileMode.Create))
        {
            await image.CopyToAsync(fileStream);
        }
        return "/images/" + image.FileName; // Trả về đường dẫn tương đối
    }
}