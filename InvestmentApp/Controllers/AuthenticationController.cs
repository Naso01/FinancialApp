using InvestmentApp.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace InvestmentApp.Controllers
{
    public class AuthenticationController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AuthenticationController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Login page
        public IActionResult Login()
        {
            return View();
        }

        // POST: Process login
        [HttpPost]
        public async Task<IActionResult> Login(string username, string password)
        {
            var user = _context.Users.FirstOrDefault(u => u.Email == username);

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

            var identity = new ClaimsIdentity(
                claims,
                "Cookies"
            );

            var principal = new ClaimsPrincipal(identity);

            await HttpContext.SignInAsync(
                "Cookies",
                    principal,
                    new AuthenticationProperties
                    {
        
                        IsPersistent = true,
                            ExpiresUtc = DateTimeOffset.UtcNow.AddMinutes(30) 
                        }
            ) ;

            return RedirectToAction("Index", "Home");
        }

        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login");
        }

        [HttpGet]
        public IActionResult SignUp()
        {
            return View();
        }

        [HttpPost]
        public IActionResult SignUp(string firstName, string lastName, string email, string password)
        {
            var existing = _context.Users.FirstOrDefault(u => u.Email == email);
            if (existing != null)
            {
                ViewBag.Error = "An account with this email already exists.";
                return View();
            }

            var user = new User
            {
                FirstName = firstName,
                LastName = lastName,
                Email = email,
                Password = password
            };

            _context.Users.Add(user);
            _context.SaveChanges();

            return RedirectToAction("Login");
        }
    }
}
