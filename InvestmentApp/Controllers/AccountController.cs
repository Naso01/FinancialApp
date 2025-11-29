using InvestmentApp.Models;
using InvestmentApp.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

/***************************************************************************************
 * Author: Hanjia Li
 * Description:
 *     This controller manages all user account–related functionality in the
 *     InvestmentApp system. It provides authenticated users with access to:
 *     - Viewing their account dashboard
 *     - Opening chequing or savings accounts
 *     - Editing account balances
 *     - Adding funds
 *     - Deleting accounts
 *     
 *     All actions require authentication and use claims-based identity to ensure that
 *     users only access and modify their own financial data.
 ***************************************************************************************/

namespace InvestmentApp.Controllers
{
    [Authorize]
    public class AccountController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly AccountService _accountService;

        // Injects the database context and account service into the controller
        public AccountController(ApplicationDbContext context, AccountService accountService)
        {
            _context = context;
            _accountService = accountService;
        }

        // GET: Displays the main account overview page (basic user info)
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

        // GET: Displays the user’s chequing or savings account details
        public IActionResult Account()
        {
            var claim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (claim == null)
                return RedirectToAction("Login", "Authentication");

            int userId = int.Parse(claim.Value);

            // Retrieve chequing and savings accounts
            var chequing = _accountService.GetAccount(userId);
            var savings = _accountService.GetSavingsAccount(userId);

            // If a savings account exists, show its interest calculation as well
            if (savings != null)
            {
                ViewBag.Interest = _accountService.CalculateSavingsInterest(savings.Balance);
                return View("Account", savings);
            }

            return View("Account", chequing);
        }

        // POST: Creates a new chequing account with an initial balance
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

        // POST: Creates a new savings account with an initial balance
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

        // POST: Updates the balance of a user's account
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

        // POST: Adds funds to an existing account
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

        // POST: Deletes the user's chequing or savings account
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
