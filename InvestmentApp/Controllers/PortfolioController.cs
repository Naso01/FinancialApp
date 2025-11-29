using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using InvestmentApp.Models;
using InvestmentApp.Services;

/***************************************************************************************
 * Author: Hanjia Li
 * Description:
 *     This controller manages all portfolio-related functionality for users within the
 *     InvestmentApp system. It allows authenticated users to:
 *     - View their existing portfolios and holdings
 *     - Create new portfolios (Managed or Self-Directed)
 *     - Edit portfolio type and optionally transfer funds
 *     - Delete entire portfolios and their holdings
 *
 *     All portfolio data is tied to a specific user. This controller ensures that
 *     portfolios are retrieved, modified, and removed securely and consistently by
 *     using Entity Framework relationships and eager loading where necessary.
 ***************************************************************************************/

public class PortfolioController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly ManagedPortfolioService _managedPortfolioService;

    // Injects database context into the controller
    public PortfolioController(ApplicationDbContext context, ManagedPortfolioService managedPortfolioService)
    {
        _context = context;
        _managedPortfolioService = managedPortfolioService;
    }

    // GET: Displays all portfolios for a specific user
    public async Task<IActionResult> Index(int userId)
    {
        var user = await _context.Users
            .Include(u => u.Portfolios)
                .ThenInclude(p => p.Holdings)
                    .ThenInclude(h => h.Stock)
            .Include(u => u.ChequingAccount)
            .FirstOrDefaultAsync(u => u.UserId == userId);

        if (user == null)
            return NotFound();

        ViewBag.UserId = userId;
        return View(user.Portfolios);
    }

    // GET: Displays form to create a new portfolio
    public async Task<IActionResult> Create(int userId)
    {
        var hasManaged = await _context.Portfolios
            .AnyAsync(p => p.UserId == userId && p.PortfolioType == PortfolioType.Managed);

        ViewBag.UserId = userId;
        ViewBag.HasManagedPortfolio = hasManaged;

        return View();
    }

    // POST: Creates a new portfolio and associates it with a user
    [HttpPost]
    public async Task<IActionResult> Create(int userId, string name, PortfolioType type)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            TempData["Error"] = "Please enter a portfolio name.";
            return RedirectToAction("Create", new { userId });
        }

        var user = await _context.Users
            .Include(u => u.Portfolios)
            .FirstOrDefaultAsync(u => u.UserId == userId);

        if (user == null)
            return NotFound();

        // Ensure user only has one Managed Portfolio
        if (type == PortfolioType.Managed && user.Portfolios.Any(p => p.PortfolioType == PortfolioType.Managed))
        {
            TempData["Error"] = "User already has a Managed Portfolio.";
            return RedirectToAction("Create", new { userId });
        }

        // Create the base portfolio record
        var portfolio = new Portfolio
        {
            UserId = userId,
            Name = name,
            PortfolioType = type,
            CreatedAt = DateTime.Now
        };

        _context.Portfolios.Add(portfolio);
        await _context.SaveChangesAsync();       // ★ PortfolioId generated here

        // If it's a Managed portfolio, initialize with default stocks
        if (type == PortfolioType.Managed)
        {
            await _managedPortfolioService.InitializeManagedPortfolio(portfolio.PortfolioId);
        }

        return RedirectToAction("Index", new { userId });
    }

    // GET: Displays the edit form for a portfolio
    public async Task<IActionResult> Edit(int id)
    {
        var portfolio = await _context.Portfolios.FindAsync(id);

        if (portfolio == null)
            return NotFound();

        return View(portfolio);
    }

    // POST: Applies edits to a portfolio, optionally transferring funds
    [HttpPost]
    public async Task<IActionResult> Edit(int id, PortfolioType type, decimal amount)
    {
        var portfolio = await _context.Portfolios
            .Include(p => p.User)
                .ThenInclude(u => u.ChequingAccount)
            .FirstOrDefaultAsync(p => p.PortfolioId == id);

        if (portfolio == null)
            return NotFound();

        // Update portfolio type
        portfolio.PortfolioType = type;

        // Transfer funds from chequing into portfolio (if amount > 0)
        if (amount > 0)
        {
            if (portfolio.User.ChequingAccount.Balance < amount)
                return BadRequest("Not enough funds.");

            portfolio.User.ChequingAccount.Balance -= amount;
        }

        await _context.SaveChangesAsync();

        return RedirectToAction("Index", new { userId = portfolio.UserId });
    }

    // POST: Deletes a portfolio and its associated holdings
    public async Task<IActionResult> Delete(int id)
    {
        var portfolio = await _context.Portfolios
            .Include(p => p.Holdings)
            .FirstOrDefaultAsync(p => p.PortfolioId == id);

        if (portfolio == null)
            return NotFound();

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
