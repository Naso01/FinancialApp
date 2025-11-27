using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using InvestmentApp.Models;
using System.Linq;
using System.Threading.Tasks;

public class PortfolioController : Controller
{
    private readonly ApplicationDbContext _context;

    public PortfolioController(ApplicationDbContext context)
    {
        _context = context;
    }

    
    public async Task<IActionResult> Index(int userId)
    {
        var user = await _context.Users
            .Include(u => u.Portfolios)
                .ThenInclude(p => p.Holdings)
                    .ThenInclude(h => h.Stock)
            .Include(u => u.ChequingAccount)
            .FirstOrDefaultAsync(u => u.UserId == userId);

        if (user == null) return NotFound();

        ViewBag.User = user;
        return View(user.Portfolios);
    }

    
    [HttpPost]
    public async Task<IActionResult> CreatePortfolio(int userId, string name, PortfolioType type)
    {
        var user = await _context.Users
            .Include(u => u.Portfolios)
            .FirstOrDefaultAsync(u => u.UserId == userId);

        if (user == null) return NotFound();

        var portfolio = new Portfolio
        {
            PortfolioType = type,
            CreatedAt = DateTime.Now,
            Holdings = new List<PortfolioHolding>()
        };

        user.Portfolios.Add(portfolio);
        await _context.SaveChangesAsync();

        return RedirectToAction("Index", new { userId });
    }

    [HttpPost]
    public async Task<IActionResult> DepositFunds(int userId, decimal amount)
    {
        var user = await _context.Users
            .Include(u => u.ChequingAccount)
            .FirstOrDefaultAsync(u => u.UserId == userId);

        if (user == null) return NotFound();

        user.ChequingAccount.Balance += amount;
        await _context.SaveChangesAsync();

        return RedirectToAction("Index", new { userId });
    }

    
    [HttpPost]
    public async Task<IActionResult> WithdrawFunds(int userId, decimal amount)
    {
        var user = await _context.Users
            .Include(u => u.ChequingAccount)
            .FirstOrDefaultAsync(u => u.UserId == userId);

        if (user == null) return NotFound();
        if (user.ChequingAccount.Balance < amount)
            return BadRequest("Insufficient funds");

        user.ChequingAccount.Balance -= amount;
        await _context.SaveChangesAsync();

        return RedirectToAction("Index", new { userId });
    }
}
