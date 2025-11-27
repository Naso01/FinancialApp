using Microsoft.AspNetCore.Mvc;
using Finnhub.Client;

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
