using InvestmentApp.Services;
using Microsoft.AspNetCore.Mvc;

namespace InvestmentApp.Controllers
{
    public class AccountController : Controller
    {
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
            _accountService.AddChequingAccount(userId, balance);
            return RedirectToAction("Account");
        }

        [HttpPost]
        public IActionResult EditAccount(int userId, decimal balance)
        {
            _accountService.EditAccount(userId, balance);
            return RedirectToAction("Account");
        }

        [HttpPost]
        public IActionResult DeleteAccount(int userId)
        {
            _accountService.DeleteAccount(userId);
            return RedirectToAction("Account");
        }
    }
}
