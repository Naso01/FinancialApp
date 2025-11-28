using InvestmentApp.Models;
using Microsoft.AspNetCore.Authorization;
using InvestmentApp.Services;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace InvestmentApp.Controllers
{
    [Authorize] // Only logged-in users can access this controller
    public class AccountController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AccountController(ApplicationDbContext context)
        {
            _context = context;
        }

        // ACCOUNT HOME
        public IActionResult Index()
        private readonly AccountService _accountService;

        public AccountController(AccountService accountService)
        {
            _accountService = accountService;
        }

        public IActionResult Account()
        {
            var idClaim = User.Claims.FirstOrDefault(c => c.Type == "UserId");

            if (idClaim == null)
            {
                return RedirectToAction("Login", "Authentication");
            }

            int userId = int.Parse(idClaim.Value);

            var account = _accountService.GetAccount(userId);
            return View(account);
        }

        [HttpPost]
        public IActionResult CreateChequing(int userId, decimal balance)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);

            var user = _context.Users.FirstOrDefault(u => u.UserId == userId);

            return View(user);
            _accountService.AddChequingAccount(userId, balance);
            return RedirectToAction("Account");
        }

        // EDIT (GET)
        public IActionResult Edit()
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
            var user = _context.Users.FirstOrDefault(u => u.UserId == userId);

            return View(user);
        }

        // EDIT (POST)
        [HttpPost]
        public IActionResult Edit(User updated)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
            var user = _context.Users.First(u => u.UserId == userId);

            user.FirstName = updated.FirstName;
            user.LastName = updated.LastName;
            user.Email = updated.Email;

            if (!string.IsNullOrEmpty(updated.Password))
                user.Password = updated.Password;

            _context.SaveChanges();

            return RedirectToAction("Index");
        }

        // DELETE (GET)
        public IActionResult Delete()
        public IActionResult EditAccount(int userId, decimal balance)
        {
            return View();
            _accountService.EditAccount(userId, balance);
            return RedirectToAction("Account");
        }

        // DELETE (POST)
        [HttpPost]
        public IActionResult DeleteConfirmed()
        public IActionResult DeleteAccount(int userId)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);

            var user = _context.Users.First(u => u.UserId == userId);

            _context.Users.Remove(user);
            _context.SaveChanges();

            // Log the user out after deleting
            return RedirectToAction("Logout", "Authentication");
            _accountService.DeleteAccount(userId);
            return RedirectToAction("Account");
        }
    }
}
