using Microsoft.EntityFrameworkCore;
using SellingWebsite.Models;
using System.ComponentModel.DataAnnotations;

namespace SellingWebsite.Models
{
    public class Product
    {
        public int Id { get; set; }
        [Required, StringLength(100)]
        public string Name { get; set; }
        [Range(0.01, 100000000.00)]

        [Precision(18, 2)]
        public decimal Price { get; set; }
        public string Description { get; set; }
        public string? ImageUrl { get; set; }
        public List<ProductImage>? Images { get; set; }
        public int CategoryId { get; set; }
        public Category? Category { get; set; }
    }
}
