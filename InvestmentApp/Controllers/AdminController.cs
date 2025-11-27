using Microsoft.AspNetCore.Mvc;

namespace InvestmentApp.Controllers
{
    public class AdminController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public IActionResult ViewUsers()
        {
            return View();
        }

        [HttpPost]
        public IActionResult VerifyPermission()
        {
            return RedirectToAction("Admin");
        }
    }
}
