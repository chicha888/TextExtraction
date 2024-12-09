using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OpenCvSharp;
using TextExtraction.Models;
using TextExtraction.ViewModels;

namespace TextExtraction.Controllers
{
    [Authorize]
    public class ProfileController : Controller
    {
        private readonly UserManager<AppUser> _userManager;

        public ProfileController(UserManager<AppUser> userManager)
        {
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier).Value;
            var currUser = await _userManager.Users
                .Include(u => u.FileHistory) // Загрузка связанных данных
                .FirstOrDefaultAsync(u => u.Id == userId);


            if (currUser == null)
            {
                return NotFound("User not found");
            }

            var model = new ProfileVM
            {
                Username = currUser.UserName,
                Email = currUser.Email,
                fileHistory = currUser.FileHistory
                    .OrderByDescending(f => f.UploadedAt)
                    .Take(10) // Берём только последние 10 записей
                    .ToList()
            };

            return View(model);
        }
    }
}
