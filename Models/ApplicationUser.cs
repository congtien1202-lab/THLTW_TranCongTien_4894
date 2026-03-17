using Microsoft.AspNetCore.Identity; using SellingWebsite.Models;

namespace SellingWebsite.Models
{
    // Kế thừa ApplicationUser để lấy sẵn Email, Password... và thêm các trường mới
    public class ApplicationUser : IdentityUser
    {
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? Address { get; set; }
        public int Age { get; set; }
        public string? AvatarUrl { get; set; }
    }
}