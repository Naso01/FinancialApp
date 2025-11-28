using InvestmentApp.Models;
using InvestmentApp.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace InvestmentApp.Controllers
{
    [Authorize]
    public class AccountController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly AccountService _accountService;

        public AccountController(ApplicationDbContext context, AccountService accountService)
        {
            _context = context;
            _accountService = accountService;
        }

        public IActionResult Account()
        {
            var claim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (claim == null)
                return RedirectToAction("Login", "Authentication");

            int userId = int.Parse(claim.Value);
            var account = _accountService.GetAccount(userId);

            return View(account);
        }

        public IActionResult Index()
        {
            return RedirectToAction("Account");
        }

        [HttpPost]
        public IActionResult CreateChequing(decimal balance)
        {
            var claim = User.FindFirst("UserId");
            if (claim == null)
                return RedirectToAction("Login", "Authentication");

            int userId = int.Parse(claim.Value);

            _accountService.AddChequingAccount(userId, balance);

            return RedirectToAction("Account");
        }

        [HttpPost]
        public IActionResult EditAccount(decimal balance)
        {
            var claim = User.FindFirst("UserId");
            if (claim == null)
                return RedirectToAction("Login", "Authentication");

            int userId = int.Parse(claim.Value);

            _accountService.EditAccount(userId, balance);

            return RedirectToAction("Account");
        }

        [HttpPost]
        public IActionResult AddFunds(decimal amount)
        {
            var claim = User.FindFirst("UserId");
            if (claim == null)
                return RedirectToAction("Login", "Authentication");

            int userId = int.Parse(claim.Value);

            _accountService.AddFunds(userId, amount);

            return RedirectToAction("Account");
        }

        [HttpPost]
        public IActionResult DeleteAccount()
        {
            var claim = User.FindFirst("UserId");
            if (claim == null)
                return RedirectToAction("Login", "Authentication");

            int userId = int.Parse(claim.Value);

            _accountService.DeleteAccount(userId);

            return RedirectToAction("Logout", "Authentication");
        }
    }
}
