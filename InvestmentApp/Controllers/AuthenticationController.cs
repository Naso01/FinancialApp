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
                new Claim(ClaimTypes.Name, user.Email),
                new Claim(ClaimTypes.NameIdentifier, user.UserId.ToString())
            };

            var identity = new ClaimsIdentity(
                claims,
                CookieAuthenticationDefaults.AuthenticationScheme
            );

            var principal = new ClaimsPrincipal(identity);

            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                principal
            );

            return RedirectToAction("Account", "Account", new {id = user.UserId});
        }

        // GET: Logout
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            return RedirectToAction("Login");
        }
    
        //Sign up
        [HttpGet]
        public IActionResult SignUp()
        {
            return View();
        }

        [HttpPost]
        public IActionResult SignUp(string firstName, string lastName, string email, string password)
        {
            // Check if email already exists
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

            // Auto-login after registration (optional)
            return RedirectToAction("Login");
        }

    }
}
