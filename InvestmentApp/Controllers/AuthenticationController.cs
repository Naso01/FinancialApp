using InvestmentApp.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

/***************************************************************************************
 * Author: Nathan Serrano
 * Description:
 *     This controller manages all authentication-related actions within the 
 *     InvestmentApp system. Its responsibilities include logging users in,
 *     logging them out, signing new users up, and creating authentication cookies 
 *     that maintain the logged-in session. It interacts directly with the database 
 *     to validate credentials and uses cookie-based authentication to manage user 
 *     identity across the application.
 ***************************************************************************************/

namespace InvestmentApp.Controllers
{
    public class AuthenticationController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AuthenticationController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Displays the login page
        public IActionResult Login()
        {
            return View();
        }

        // POST: Verifies login credentials and authenticates the user
        [HttpPost]
        public async Task<IActionResult> Login(string email, string password)
        {
            var user = _context.Users.FirstOrDefault(u => u.Email == email);

            if (user == null)
            {
                ViewBag.Error = "No account found with that email.";
                ViewBag.ShowSignup = true;
                return View();
            }

            if (user.Password != password)
            {
                ViewBag.Error = "Incorrect password.";
                ViewBag.ShowSignup = true;
                return View();
            }

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.UserId.ToString()),
                new Claim(ClaimTypes.Name, user.Email)
            };

            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var principal = new ClaimsPrincipal(identity);

            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                principal,
                new AuthenticationProperties
                {
                    IsPersistent = true,
                    ExpiresUtc = DateTimeOffset.UtcNow.AddMinutes(30)
                }
            );

            return RedirectToAction("Index", "Home");
        }

        // Logs the user out by clearing the authentication cookie
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login");
        }

        // GET: Displays signup form
        [HttpGet]
        public IActionResult SignUp()
        {
            return View();
        }

        // POST: Creates a new user account and logs the user in automatically
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SignUp(User newUser)
        {
            if (!ModelState.IsValid)
            {
                return View(newUser);
            }

            // Check if email already exists
            var existingUser = _context.Users.FirstOrDefault(u => u.Email == newUser.Email);
            if (existingUser != null)
            {
                ViewBag.Error = "Email already exists!";
                return View(newUser);
            }

            // Save user
            _context.Users.Add(newUser);
            _context.SaveChanges();

            // Auto-login
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, newUser.UserId.ToString()),
                new Claim(ClaimTypes.Name, newUser.FirstName),
                new Claim(ClaimTypes.Email, newUser.Email)
            };

            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var principal = new ClaimsPrincipal(identity);

            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

            // Redirect to User Profile (dashboard)
            return RedirectToAction("Index", "Home");
        }
    }
}
