// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
#nullable disable
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Hosting; // THÊM THƯ VIỆN NÀY ĐỂ DÙNG IWebHostEnvironment
using System.IO; // THÊM THƯ VIỆN NÀY ĐỂ XỬ LÝ FILE
using SellingWebsite.Data;
using SellingWebsite.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System;
using System.ComponentModel.DataAnnotations;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;

namespace SellingWebsite.Areas.Identity.Pages.Account.Manage
{
    public class IndexModel : PageModel
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly SellingWebsite.Data.ApplicationDbContext _context;
        private readonly IWebHostEnvironment _env; // THÊM BIẾN MÔI TRƯỜNG ĐỂ LƯU FILE

        public IndexModel(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            SellingWebsite.Data.ApplicationDbContext context,
            IWebHostEnvironment env) // TIÊM VÀO CONSTRUCTOR
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _context = context;
            _env = env; // GÁN GIÁ TRỊ
        }

        public string Username { get; set; }
        public List<Order> UserOrders { get; set; }

        [TempData]
        public string StatusMessage { get; set; }

        [BindProperty]
        public InputModel Input { get; set; }

        public class InputModel
        {
            [Phone]
            [Display(Name = "Phone number")]
            public string PhoneNumber { get; set; }

            public string AvatarUrl { get; set; }

            [Display(Name = "Họ")]
            public string FirstName { get; set; }

            [Display(Name = "Tên")]
            public string LastName { get; set; }

            [Display(Name = "Ảnh đại diện")]
            public IFormFile ProfilePicture { get; set; }
        }

        private async Task LoadAsync(ApplicationUser user)
        {
            var userName = await _userManager.GetUserNameAsync(user);
            var phoneNumber = await _userManager.GetPhoneNumberAsync(user);

            Username = userName;

            Input = new InputModel
            {
                PhoneNumber = phoneNumber,
                FirstName = user.FirstName,
                LastName = user.LastName,
                AvatarUrl = user.AvatarUrl // Kéo link ảnh từ DB ra để hiển thị
            };
        }

        public async Task<IActionResult> OnGetAsync()
        {
            var user = await _userManager.GetUserAsync(User) as ApplicationUser;
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            await LoadAsync(user);
            UserOrders = _context.Orders.Where(o => o.UserId == user.Id).OrderByDescending(o => o.OrderDate).ToList();
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            if (!ModelState.IsValid)
            {
                await LoadAsync(user);
                return Page();
            }

            var phoneNumber = await _userManager.GetPhoneNumberAsync(user);
            if (Input.PhoneNumber != phoneNumber)
            {
                var setPhoneResult = await _userManager.SetPhoneNumberAsync(user, Input.PhoneNumber);
                if (!setPhoneResult.Succeeded)
                {
                    StatusMessage = "Unexpected error when trying to set phone number.";
                    return RedirectToPage();
                }
            }

            // --- LƯU HỌ VÀ TÊN ---
            if (Input.FirstName != user.FirstName || Input.LastName != user.LastName)
            {
                user.FirstName = Input.FirstName;
                user.LastName = Input.LastName;

                var updateResult = await _userManager.UpdateAsync(user);
                if (!updateResult.Succeeded)
                {
                    StatusMessage = "Lỗi khi cập nhật Họ và Tên vào cơ sở dữ liệu.";
                    return RedirectToPage();
                }
            }

            // --- BẮT ĐẦU LƯU ẢNH (Phần bạn bị thiếu) ---
            if (Input.ProfilePicture != null)
            {
                string uploadsFolder = Path.Combine(_env.WebRootPath, "images", "avatars");

                if (!Directory.Exists(uploadsFolder))
                {
                    Directory.CreateDirectory(uploadsFolder);
                }

                string uniqueFileName = Guid.NewGuid().ToString() + "_" + Input.ProfilePicture.FileName;
                string filePath = Path.Combine(uploadsFolder, uniqueFileName);

                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await Input.ProfilePicture.CopyToAsync(fileStream);
                }

                user.AvatarUrl = "/images/avatars/" + uniqueFileName;
                await _userManager.UpdateAsync(user);
            }
            // --- KẾT THÚC LƯU ẢNH ---

            await _signInManager.RefreshSignInAsync(user);

            StatusMessage = "Your profile has been updated";
            return RedirectToPage();
        }
    }
}