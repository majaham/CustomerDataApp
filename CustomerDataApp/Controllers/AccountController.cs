using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using CustomerDataApp.Models;
using System.Linq;
using System.Threading.Tasks;

namespace CustomerDataApp.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;
        
        private readonly ILogger<AccountController> _logger;

        public AccountController(SignInManager<IdentityUser> signInManager,
            ILogger<AccountController> logger,
            UserManager<IdentityUser> userManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;           
            //Add admins if they do not exists.
            CreateAdministratorAsync("admin@admin").GetAwaiter().GetResult();
            CreateAdministratorAsync("admin2@admin2").GetAwaiter().GetResult();
           
            _logger = logger;
        }

        private async Task CreateAdministratorAsync(string admin)
        {
            var user = new IdentityUser { UserName = admin, Email = admin };
            var currentUser = await _userManager.FindByEmailAsync(admin);
            if (currentUser == null)
            {
                await _userManager.CreateAsync(user, "P@ssword01");
                currentUser = await _userManager.FindByEmailAsync(admin);
                await _userManager.AddToRoleAsync(currentUser, "Admin");               
            }
          
        }

        [HttpGet]
        public async Task<IActionResult> LoginAsync(string returnUrl = null)
        {
            if (!string.IsNullOrEmpty(ViewBag.ErrorMessage))
            {
                ModelState.AddModelError(string.Empty, ViewBag.ErrorMessage);
            }

            returnUrl ??= Url.Content("~/");
            // Clear the existing external cookie to ensure a clean login process
            await HttpContext.SignOutAsync(IdentityConstants.ExternalScheme);

            ViewBag.ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
            ViewBag.ReturnUrl = returnUrl;

            return View();
        }

        [HttpPost()]
        public async Task<IActionResult> Login(LoginModel loginModel, string returnUrl = null)
        {
            returnUrl ??= Url.Content("~/");

            ViewBag.ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();

            if (ModelState.IsValid)
            {
                // This doesn't count login failures towards account lockout
                // To enable password failures to trigger account lockout, set lockoutOnFailure: true
                var result = await _signInManager.PasswordSignInAsync(loginModel.Email, loginModel.Password, loginModel.RememberMe, lockoutOnFailure: false);
                if (result.Succeeded)
                {
                    _logger.LogInformation("User logged in.");
                    return LocalRedirect("/Customer/Index");
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Invalid login attempt.");
                    return View();
                }
            }
            return View();
        }
        [Authorize(Roles = "Admin")]
        [HttpGet]
        public async Task<IActionResult> Register(string returnUrl = null)
        {
            ViewBag.ReturnUrl = returnUrl;
            ViewBag.ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();

            return View();
        }
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> Register(RegisterModel registerModel, string returnUrl = null)
        {
            returnUrl ??= Url.Content("~/");
            ViewBag.ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
            if (ModelState.IsValid)
            {
                var user = new IdentityUser { UserName = registerModel.Email, Email = registerModel.Email };
                var result = await _userManager.CreateAsync(user, registerModel.Password);
                if (result.Succeeded)
                {
                    _logger.LogInformation("User created a new account with password.");
                  
                    await _signInManager.SignInAsync(user, isPersistent: false);
                    return LocalRedirect("/Customer/Index");
                }
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }
            // If we got this far, something failed, redisplay form
            return View();
        }
       
        [HttpPost]
        public async Task<IActionResult> Logout(string returnUrl = null)
        {
            await _signInManager.SignOutAsync();
            _logger.LogInformation("User logged out.");

            return View("Login");         
        }
    }
}
