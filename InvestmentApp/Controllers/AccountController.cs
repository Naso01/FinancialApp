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

        public IActionResult Index()
        {
            var claim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (claim == null)
                return RedirectToAction("Login", "Authentication");

            int userId = int.Parse(claim.Value);
            var user = _context.Users.Find(userId);

            if (user == null)
                return RedirectToAction("Login", "Authentication");

            return View(user);
        }
        public IActionResult Account()
        {
            var claim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (claim == null)
                return RedirectToAction("Login", "Authentication");

            int userId = int.Parse(claim.Value);
            var chequing = _accountService.GetAccount(userId);
            var savings = _accountService.GetSavingsAccount(userId);

            if (savings != null)
            {
                ViewBag.Interest = _accountService.CalculateSavingsInterest(savings.Balance);
                return View("Account", savings);
            }
            return View("Account", chequing);
        }

        [HttpPost]
        public IActionResult CreateChequing(decimal balance)
        {
            var claim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (claim == null)
                return RedirectToAction("Login", "Authentication");

            int userId = int.Parse(claim.Value);
            _accountService.AddChequingAccount(userId, balance);
            return RedirectToAction("Account");
        }

        [HttpPost]
        public IActionResult CreateSavings(decimal balance)
        {
            var claim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (claim == null)
                return RedirectToAction("Login", "Authentication");

            int userId = int.Parse(claim.Value);
            _accountService.AddSavingsAccount(userId, balance);

            return RedirectToAction("Account");
        }


        [HttpPost]
        public IActionResult EditAccount(decimal balance)
        {
            var claim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (claim == null)
                return RedirectToAction("Login", "Authentication");

            int userId = int.Parse(claim.Value);
            _accountService.EditAccount(userId, balance);
            return RedirectToAction("Account");
        }

        [HttpPost]
        public IActionResult AddFunds(decimal amount)
        {
            var claim = User.FindFirst(ClaimTypes.NameIdentifier); 
            if (claim == null)
                return RedirectToAction("Login", "Authentication");

            int userId = int.Parse(claim.Value);
            _accountService.AddFunds(userId, amount);
            return RedirectToAction("Account");
        }

        [HttpPost]
        public IActionResult DeleteAccount()
        {
            var claim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (claim == null)
                return RedirectToAction("Login", "Authentication");

            int userId = int.Parse(claim.Value);
            _accountService.DeleteAccount(userId);
            return RedirectToAction("Account");
        }
    }
}