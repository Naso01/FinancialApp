using Microsoft.AspNetCore.Mvc;

namespace InvestmentApp.Controllers
{
    public class PortfolioController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public IActionResult EditPortfolio(int id)
        {
            return RedirectToAction("Portfolio");
        }

        [HttpPost]
        public IActionResult DeletePortfolio(int id)
        {
            return RedirectToAction("Portfolio");
        }

        [HttpPost]
        public IActionResult AddPortfolio()
        {
            return RedirectToAction("Portfolio");
        }
    }
}
