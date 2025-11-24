using Microsoft.AspNetCore.Mvc;

namespace InvestmentApp.Controllers
{
    public class APIController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
