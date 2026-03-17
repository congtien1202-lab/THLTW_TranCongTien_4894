using Microsoft.EntityFrameworkCore;
using SellingWebsite.Models;
using SellingWebsite.Repositories;

namespace NguyenDangAn_2652.Repositories
{
    public class EFCategoryRepository : ICategoryRepository
    {
        private readonly ApplicationDbContext _context;
        public EFCategoryRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        // 1. Lấy tất cả danh mục
        public async Task<IEnumerable<Category>> GetAllAsync()
        {
            // Lưu ý: Đảm bảo trong ApplicationDbContext bạn đã khai báo: 
            // public DbSet<Category> Categories { get; set; }
            return await _context.Categories.ToListAsync();
        }

        // 2. Lấy danh mục theo ID
        public async Task<Category> GetByIdAsync(int id)
        {
            return await _context.Categories.FindAsync(id);
        }

        // 3. Thêm mới danh mục
        public async Task AddAsync(Category category)
        {
            await _context.Categories.AddAsync(category);
            await _context.SaveChangesAsync();
        }

        // 4. Cập nhật danh mục
        public async Task UpdateAsync(Category category)
        {
            _context.Categories.Update(category);
            await _context.SaveChangesAsync();
        }

        // 5. Xóa danh mục
        public async Task DeleteAsync(int id)
        {
            var category = await _context.Categories.FindAsync(id);
            if (category != null)
            {
                _context.Categories.Remove(category);
                await _context.SaveChangesAsync();
            }
        }
    }
}
