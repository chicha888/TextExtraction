using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using TextExtraction.Data;
using TextExtraction.Models;
using TextExtraction.ViewModels;

namespace TextExtraction.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;

        public AccountController(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }


        public IActionResult Login()
        {
            var responce = new LoginVM();
            return View(responce);
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginVM login)
        {
            if (!ModelState.IsValid) return View(login);

            var user = await _userManager.FindByEmailAsync(login.EmailAddress);
            if (user != null)
            {
                var passwordCheck = await _userManager.CheckPasswordAsync(user, login.Password);
                if (passwordCheck)
                {
                    var result = await _signInManager.PasswordSignInAsync(user, login.Password, false, false);
                    if (result.Succeeded)
                    {
                        return RedirectToAction("Index", "Home");
                    }
                    else
                    {
                        TempData["Error"] = "Wrong cridentials. Try again";
                        return View(login);
                    }
                }
            }

            TempData["Error"] = "Wrong cridentials. Try again";
            return View(login);
        }

        public IActionResult Register()
        {
            var responce = new RegisterVM();
            return View(responce);
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterVM registerVM)
        {
            if (!ModelState.IsValid) return View(registerVM);

            var user = await _userManager.FindByEmailAsync(registerVM.Email);
            if (user != null)
            {
                TempData["Error"] = "This email is already in use!";
                return View(registerVM);
            }

            var newUser = new AppUser 
            { 
                UserName = registerVM.Username,
                Email = registerVM.Email,
                CreatedDate = DateTime.Now,
            };

            var registerNewUser = await _userManager.CreateAsync(newUser, registerVM.Password);
            if (registerNewUser.Succeeded) 
            {
                await _userManager.AddToRoleAsync(newUser, UserRoles.User);
                return RedirectToAction("Index", "Home");
            }

            foreach (var error in registerNewUser.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }

            return View(registerVM);

        }

        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }
    }
}
