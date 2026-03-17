using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using SellingWebsite.Models; // Để nhận diện Product, Category, Order...

namespace SellingWebsite.Data // <-- Sửa lại thành Data để khớp với toàn dự án
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        // Khôi phục lại toàn bộ các bảng của cửa hàng
        public DbSet<Product> Products { get; set; }
        public DbSet<Category> Categories { get; set; }
        // public DbSet<ProductImage> ProductImages { get; set; } // Bỏ comment nếu bạn có bảng này

        // Bảng đơn hàng mới thêm
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderDetail> OrderDetails { get; set; }
    }
}