using System.ComponentModel.DataAnnotations.Schema;
using SellingWebsite.Models;

namespace SellingWebsite.Models
{
    public class OrderDetail
    {
        public int Id { get; set; }

        public int OrderId { get; set; }
        [ForeignKey("OrderId")]
        public Order Order { get; set; } // Liên kết ngược lại với hóa đơn tổng

        public int ProductId { get; set; }
        [ForeignKey("ProductId")]
        public Product Product { get; set; } // Liên kết với sản phẩm được mua

        public int Quantity { get; set; } // Số lượng (ví dụ: đặt 1 hoặc 2 suất chụp)
        public decimal Price { get; set; } // Giá tại thời điểm mua
    }
}