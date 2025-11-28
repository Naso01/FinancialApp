using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using InvestmentApp.Models;

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

        ViewBag.UserId = userId;
        return View(user.Portfolios);
    }
    public IActionResult Create(int userId)
    {
        ViewBag.UserId = userId;
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Create(int userId, PortfolioType type)
    {
        var user = await _context.Users
            .Include(u => u.Portfolios)
            .FirstOrDefaultAsync(u => u.UserId == userId);

        if (user == null)
            return NotFound();

        var portfolio = new Portfolio
        {
            PortfolioType = type,
            CreatedAt = DateTime.Now,
            Holdings = new List<PortfolioHolding>()
        };

        user.Portfolios.Add(portfolio);
        await _context.SaveChangesAsync();

        return RedirectToAction("Index", new { userId = userId });
    }


    public async Task<IActionResult> Edit(int id)
    {
        var portfolio = await _context.Portfolios.FindAsync(id);
        if (portfolio == null) return NotFound();

        return View(portfolio);
    }

    [HttpPost]
    public async Task<IActionResult> Edit(int id, PortfolioType type)
    {
        var p = await _context.Portfolios.FindAsync(id);
        if (p == null) return NotFound();

        p.PortfolioType = type;

        await _context.SaveChangesAsync();

        return RedirectToAction("Index", new { userId = p.UserId });
    }

    public async Task<IActionResult> Delete(int id)
    {
        var portfolio = await _context.Portfolios
            .Include(p => p.Holdings)
            .FirstOrDefaultAsync(p => p.PortfolioId == id);

        if (portfolio == null) return NotFound();

        // delete holdings first
        _context.PortfolioHoldings.RemoveRange(portfolio.Holdings);

        _context.Portfolios.Remove(portfolio);
        await _context.SaveChangesAsync();

        return RedirectToAction("Index", new { userId = portfolio.UserId });
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
