using InvestmentApp.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;

/***************************************************************************************
 * Author: Hanjia Li
 * Description:
 *     This controller handles administrative login and dashboard functionality for the
 *     InvestmentApp system. It provides:
 *     - Admin authentication (session-based)
 *     - Viewing all registered users
 *     - Logging out and clearing admin session data
 * 
 *     This controller does not use ASP.NET Identity—administrators are authenticated
 *     through the custom AdminService and a session flag ("AdminLoggedIn").
 ***************************************************************************************/

namespace InvestmentApp.Controllers
{
    public class AdminController : Controller
    {
        private readonly AdminService _adminService;

        // Injects the admin service, which handles validation and user retrieval
        public AdminController(AdminService adminService)
        {
            _adminService = adminService;
        }

        // GET: Displays the admin login page
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        // POST: Validates admin login credentials
        [HttpPost]
        public IActionResult Login(string username, string password)
        {
            var admin = _adminService.ValidateLogin(username, password);

            // If credentials are invalid, return login screen with error message
            if (admin == null)
            {
                ViewBag.Error = "Invalid username or password.";
                return View();
            }

            // Store session values to mark admin as authenticated
            HttpContext.Session.SetString("AdminLoggedIn", "true");
            HttpContext.Session.SetString("AdminUsername", admin.Username);

            return RedirectToAction("Index");
        }

        // GET: Admin dashboard showing all users in the system
        public IActionResult Index()
        {
            // Ensure the admin is logged in
            if (HttpContext.Session.GetString("AdminLoggedIn") != "true")
                return RedirectToAction("Login");

            var users = _adminService.GetAllUsers();
            return View("Admin", users);
        }

        // GET: Logs the admin out and clears all session data
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login");
        }
    }
}
