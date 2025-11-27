using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using InvestmentApp.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace InvestmentApp.Controllers
{
    public class StockController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly StockService _stockService;

        public StockController(ApplicationDbContext context, StockService stockService)
        {
            _context = context;
            _stockService = stockService;
        }

        // Display all portfolios for a user with live stock prices
        public async Task<IActionResult> Index(int userId)
        {
            var user = await _context.Users
                .Include(u => u.ChequingAccount)
                .Include(u => u.Portfolios)
                    .ThenInclude(p => p.Holdings)
                        .ThenInclude(h => h.Stock)
                .FirstOrDefaultAsync(u => u.UserId == userId);

            if (user == null) return NotFound();

            var portfoliosWithPrices = new Dictionary<Portfolio, List<(PortfolioHolding holding, Stock stock)>>();

            foreach (var portfolio in user.Portfolios)
            {
                var holdingsWithPrices = new List<(PortfolioHolding, Stock)>();
                foreach (var holding in portfolio.Holdings)
                {
                    var stock = await _stockService.GetStockAsync(holding.Stock.Symbol);
                    holdingsWithPrices.Add((holding, stock));
                }
                portfoliosWithPrices[portfolio] = holdingsWithPrices;
            }

            ViewBag.User = user;
            ViewBag.AllStocks = await _context.Stocks.ToListAsync(); 
            return View(portfoliosWithPrices);
        }

        [HttpPost]
        public async Task<IActionResult> BuyStock(int userId, int portfolioId, int stockId, decimal quantity)
        {
            var user = await _context.Users
                .Include(u => u.ChequingAccount)
                .Include(u => u.Portfolios)
                    .ThenInclude(p => p.Holdings)
                        .ThenInclude(h => h.Stock)
                .FirstOrDefaultAsync(u => u.UserId == userId);

            if (user == null) return NotFound();

            var portfolio = user.Portfolios.FirstOrDefault(p => p.PortfolioId == portfolioId);
            if (portfolio == null) return BadRequest("Portfolio not found");

            var stock = await _context.Stocks.FirstOrDefaultAsync(s => s.StockId == stockId);
            if (stock == null) return BadRequest("Stock not found");

            var currentPrice = (await _stockService.GetStockAsync(stock.Symbol)).Price;
            var cost = currentPrice * quantity;

            if (user.ChequingAccount.Balance < cost)
                return BadRequest("Insufficient funds");

            user.ChequingAccount.Balance -= cost;

            var holding = portfolio.Holdings.FirstOrDefault(h => h.StockId == stockId);
            if (holding == null)
            {
                portfolio.Holdings.Add(new PortfolioHolding
                {
                    StockId = stockId,
                    Quantity = quantity,
                    AvgPrice = currentPrice
                });
            }
            else
            {
                holding.AvgPrice =
                    (holding.AvgPrice * holding.Quantity + currentPrice * quantity)
                    / (holding.Quantity + quantity);
                holding.Quantity += quantity;
            }

            await _context.SaveChangesAsync();
            return RedirectToAction("Index", new { userId });
        }

        [HttpPost]
        public async Task<IActionResult> SellStock(int userId, int portfolioId, int stockId, decimal quantity)
        {
            var user = await _context.Users
                .Include(u => u.ChequingAccount)
                .Include(u => u.Portfolios)
                    .ThenInclude(p => p.Holdings)
                        .ThenInclude(h => h.Stock)
                .FirstOrDefaultAsync(u => u.UserId == userId);

            if (user == null) return NotFound();

            var portfolio = user.Portfolios.FirstOrDefault(p => p.PortfolioId == portfolioId);
            if (portfolio == null) return BadRequest("Portfolio not found");

            var holding = portfolio.Holdings.FirstOrDefault(h => h.StockId == stockId);
            if (holding == null || holding.Quantity < quantity)
                return BadRequest("Not enough stocks to sell");

            var stock = await _context.Stocks.FirstOrDefaultAsync(s => s.StockId == stockId);
            var currentPrice = (await _stockService.GetStockAsync(stock.Symbol)).Price;

            user.ChequingAccount.Balance += currentPrice * quantity;

            holding.Quantity -= quantity;
            if (holding.Quantity <= 0)
                portfolio.Holdings.Remove(holding);

            await _context.SaveChangesAsync();
            return RedirectToAction("Index", new { userId });
        }
    }
}
