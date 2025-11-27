using Microsoft.AspNetCore.Mvc;

namespace InvestmentApp.Controllers
{
    public class StockController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
