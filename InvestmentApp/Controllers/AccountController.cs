using Microsoft.AspNetCore.Mvc;

namespace InvestmentApp.Controllers
{
    public class AccountController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public IActionResult EditAccount(int id)
        {
            return RedirectToAction("Account");
        }

        [HttpPost]
        public IActionResult DeleteAccount(int id)
        {
            return RedirectToAction("Account");
        }
    }
}
