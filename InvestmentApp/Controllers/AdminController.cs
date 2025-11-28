using InvestmentApp.Services;
using Microsoft.AspNetCore.Mvc;

namespace InvestmentApp.Controllers
{
    public class AdminController : Controller
    {
        private readonly AdminService _adminService;

        public AdminController(AdminService adminService)
        {
            _adminService = adminService;
        }

        //login
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        //validation for login
        [HttpPost]
        public IActionResult Login(string username, string password)
        {
            var admin = _adminService.ValidateLogin(username, password);

            if (admin == null)
            {
                ViewBag.Error = "Invalid username or password.";
                return View();
            }

            //save cookie session
            HttpContext.Session.SetString("AdminLoggedIn", "true");
            HttpContext.Session.SetString("AdminUsername", admin.Username);

            return RedirectToAction("Index");
        }

        //admin home
        public IActionResult Index()
        {
            if (HttpContext.Session.GetString("AdminLoggedIn") != "true")
                return RedirectToAction("Login");

            var users = _adminService.GetAllUsers();
            return View("Admin", users);
        }

        //logout
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login");
        }
    }
}
