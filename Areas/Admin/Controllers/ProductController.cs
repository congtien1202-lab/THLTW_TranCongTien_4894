using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using SellingWebsite.Models; // Chứa các Model của bạn
using SellingWebsite.Repositories; // Chứa IProductRepository

namespace SellingWebsite.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")] // Yêu cầu tài khoản phải có quyền Admin mới được vào
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
        // Hiển thị danh sách sản phẩm
        public async Task<IActionResult> Index()
        {
            var products = await _productRepository.GetAllAsync();
            return View(products);
        }
        // Hiển thị form thêm sản phẩm mới
        public async Task<IActionResult> Add()
        {
            // Lấy danh sách danh mục từ Database (giả sử bạn đã có _categoryRepository)
            var categories = await _categoryRepository.GetAllAsync();

            // Truyền danh sách vào ViewBag để View có thể sử dụng
            ViewBag.Categories = new SelectList(categories, "Id", "Name");

            return View();
        }
        // Xử lý thêm sản phẩm mới
        [HttpPost]
        public async Task<IActionResult> Add(Product product, IFormFile Image)
        {
            if (!ModelState.IsValid)
            {
                // Đoạn code này sẽ giúp bạn liệt kê tất cả lỗi ra cửa sổ "Output" hoặc "Debug" của Visual Studio
                var errors = ModelState.Values.SelectMany(v => v.Errors);
                foreach (var error in errors)
                {
                    Console.WriteLine(error.ErrorMessage);
                }

                // Load lại danh mục để View không bị lỗi trắng Dropdown
                var Categories = await _categoryRepository.GetAllAsync();
                ViewBag.Categories = new SelectList(Categories, "Id", "Name");

                return View(product);
            }
            if (ModelState.IsValid)
            {
                if (Image != null)
                {
                    // Lưu hình ảnh và gán đường dẫn vào thuộc tính ImageUrl của Model
                    product.ImageUrl = await SaveImage(Image);
                }

                await _productRepository.AddAsync(product);
                return RedirectToAction(nameof(Index));
            }

            // Nếu có lỗi (ModelState không hợp lệ), phải load lại danh mục để người dùng chọn lại
            var categories = await _categoryRepository.GetAllAsync();
            ViewBag.Categories = new SelectList(categories, "Id", "Name");
            return View(product);
        }

        private async Task<string> SaveImage(IFormFile image)
        {
            // Tạo tên file duy nhất để không bị ghi đè nếu trùng tên
            var fileName = Guid.NewGuid().ToString() + Path.GetExtension(image.FileName);
            var savePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Images", fileName);

            using (var fileStream = new FileStream(savePath, FileMode.Create))
            {
                await image.CopyToAsync(fileStream);
            }
            return "/Images/" + fileName;
        }
        // Hiển thị thông tin chi tiết sản phẩm
        public async Task<IActionResult> Display(int id)
        {
            var product = await _productRepository.GetByIdAsync(id);
            if (product == null)
            {
                return NotFound();
            }
            return View(product);
        }
        // Hiển thị form cập nhật sản phẩm
        public async Task<IActionResult> Update(int id)
        {
            var product = await _productRepository.GetByIdAsync(id);
            if (product == null)
            {
                return NotFound();
            }
            var categories = await _categoryRepository.GetAllAsync();
            ViewBag.Categories = new SelectList(categories, "Id", "Name",
            product.CategoryId);
            return View(product);
        }
        // Xử lý cập nhật sản phẩm
        [HttpPost]
        [ValidateAntiForgeryToken] // Thêm để bảo mật chống giả mạo request
        public async Task<IActionResult> Update(int id, Product product, IFormFile imageUrl)
        {
            if (imageUrl == null)
            {
                // Nếu dòng này hiện ra trong cửa sổ Output, nghĩa là tham số imageUrl bị trống
                System.Diagnostics.Debug.WriteLine("--- LỖI: Controller nhận được request nhưng imageUrl vẫn NULL ---");
            }
            else
            {
                System.Diagnostics.Debug.WriteLine("--- THÀNH CÔNG: Đã nhận được file: " + imageUrl.FileName + " ---");
            }
            
            // 1. Loại bỏ kiểm tra ImageUrl vì chúng ta xử lý thủ công
            ModelState.Remove("ImageUrl");

            if (id != product.Id) return NotFound();

            if (ModelState.IsValid)
            {
                // 2. Lấy dữ liệu thực tế từ DB lên
                var existingProduct = await _productRepository.GetByIdAsync(id);
                if (existingProduct == null) return NotFound();

                // 3. Xử lý logic ảnh
                if (imageUrl != null)
                {
                    // Lưu ảnh mới và gán đường dẫn
                    existingProduct.ImageUrl = await SaveImage(imageUrl);
                }
                // Nếu imageUrl == null, existingProduct.ImageUrl vẫn giữ nguyên giá trị cũ từ DB

                // 4. Cập nhật các thông tin còn lại vào đối tượng 'existingProduct'
                existingProduct.Name = product.Name;
                existingProduct.Price = product.Price;
                existingProduct.Description = product.Description;
                existingProduct.CategoryId = product.CategoryId;

                // 5. Lưu đối tượng đã được cập nhật
                await _productRepository.UpdateAsync(existingProduct);

                return RedirectToAction(nameof(Index));
            }

            // Nếu có lỗi ModelState, load lại danh mục để hiển thị lại View
            var categories = await _categoryRepository.GetAllAsync();
            ViewBag.Categories = new SelectList(categories, "Id", "Name", product.CategoryId);
            return View(product);
        }
        // Hàm hiện trang xác nhận (GET)
        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            var product = await _productRepository.GetByIdAsync(id);
            if (product == null) return NotFound();
            return View(product);
        }

        // Hàm xử lý xóa (POST)
        // Quan trọng: Thêm [ActionName("DeleteConfirmed")] để khớp với asp-action trong View của bạn
        [HttpPost, ActionName("DeleteConfirmed")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _productRepository.DeleteAsync(id);
            return RedirectToAction(nameof(Index));
        }
    }
}