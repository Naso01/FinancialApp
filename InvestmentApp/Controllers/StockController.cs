using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using InvestmentApp.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;

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

        private int? GetCurrentUserId()
        {
            // Check if the user is authenticated
            if (User.Identity.IsAuthenticated)
            {
                // Find the NameIdentifier claim (which should hold the UserId)
                var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == System.Security.Claims.ClaimTypes.NameIdentifier);

                if (userIdClaim != null && int.TryParse(userIdClaim.Value, out int userId))
                {
                    return userId;
                }
            }
            return null; // Not authenticated or claim is missing/invalid
        }

        // Display user portfolios with live prices
        [Microsoft.AspNetCore.Authorization.Authorize]
        public async Task<IActionResult> Index()
        {
            var userId = GetCurrentUserId();
            if (userId == null) return RedirectToAction("Login", "Authentication");
            // Get the user and include portfolios, holdings, and stocks
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
                    var stock = holding.Stock; // get Stock entity from DB

                    if (stock != null)
                    {
                        // Fetch live price from Finnhub
                        var liveQuote = await _stockService.GetStockAsync(stock.Symbol);
                        stock.Price = liveQuote.Price; // update entity with live price

                        holdingsWithPrices.Add((holding, stock));
                    }
                }

                portfoliosWithPrices[portfolio] = holdingsWithPrices;
            }

            ViewBag.User = user;
            ViewBag.AllStocks = await _context.Stocks.ToListAsync(); // for Buy form

            return View(portfoliosWithPrices);
        }

        // SEARCH PAGE (GET)
        public async Task<IActionResult> Search()
        {
            
            var userId = GetCurrentUserId();

            if (userId == null) return RedirectToAction("Login", "Authentication");

            var user = await _context.Users
                .Include(u => u.Portfolios)
                .FirstOrDefaultAsync(u => u.UserId == userId);

            ViewBag.User = user;
            ViewBag.UserId = user.UserId;

            
            if (!user.Portfolios.Any())
            {
               
                
            }

            return View();
        }

        // SEARCH PAGE (POST)
        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Search(string query)
        {
            // Retrieve the user to populate the Buy forms in the view
            
            var userId = GetCurrentUserId();

            if (userId == null)
            {
                
                return RedirectToAction("Login", "Authentication");
            }

            var user = await _context.Users
                .Include(u => u.Portfolios)
                .FirstOrDefaultAsync(u => u.UserId == userId);

            ViewBag.User = user;

            if (string.IsNullOrWhiteSpace(query))
            {
                ViewBag.Results = new List<Stock>();
            }
            else
            {
                // Finnhub API search
                var searchResults = await _stockService.SearchStockAsync(query);

                var stocks = new List<Stock>();

                var firstResult = searchResults.FirstOrDefault();

                foreach (var result in searchResults)
                {
                    // Check if already in DB
                    
                    var stock = await _context.Stocks.FirstOrDefaultAsync(s => s.Symbol == result.Symbol);
                    if (stock == null)
                    {
                        stock = new Stock
                        {
                            Symbol = result.Symbol,
                            CompanyName = result.Description ?? result.Symbol, // fallback if description missing
                            LastUpdated = System.DateTime.Now
                        };
                        _context.Stocks.Add(stock);
                        await _context.SaveChangesAsync();
                    }

                    // Fetch live price
                    var liveQuote = await _stockService.GetStockAsync(stock.Symbol);
                    stock.Price = liveQuote.Price;

                    stocks.Add(stock);
                    
                }

                ViewBag.Results = stocks;
            }

            return View();
        }

        // BUY STOCK FROM SEARCH
        [HttpPost]
public async Task<IActionResult> BuyFromSearch(int userId, int portfolioId, string symbol, decimal quantity)
{
    // Get the logged-in user with portfolio and holdings
    var user = await _context.Users
        .Include(u => u.ChequingAccount)
        .Include(u => u.Portfolios)
            .ThenInclude(p => p.Holdings)
                .ThenInclude(h => h.Stock)
        .FirstOrDefaultAsync(u => u.UserId == userId);

    if (user == null)
        return NotFound("User not found.");

    var portfolio = user.Portfolios.FirstOrDefault(p => p.PortfolioId == portfolioId);
    if (portfolio == null)
        return BadRequest("Portfolio not found.");

    // Check if stock exists in DB
    var stock = await _context.Stocks.FirstOrDefaultAsync(s => s.Symbol == symbol);

    if (stock == null)
    {
        // Fetch details from Finnhub (symbol, company name, latest price)
        var stockInfo = await _stockService.GetStockDetailsAsync(symbol);

        if (stockInfo == null)
            return BadRequest("Stock not found.");

        stock = new Stock
        {
            Symbol = stockInfo.Symbol,
            CompanyName = stockInfo.CompanyName,
            Price = stockInfo.Price,
            LastUpdated = DateTime.Now
        };

        _context.Stocks.Add(stock);
        await _context.SaveChangesAsync(); // Save to generate StockId
    }
    else
    {
        // Update price in DB to current Finnhub price
        var quote = await _stockService.GetStockAsync(symbol);
        stock.Price = quote.Price;
        stock.LastUpdated = DateTime.Now;
        await _context.SaveChangesAsync();
    }

    var cost = stock.Price * quantity;
    if (user.ChequingAccount.Balance < cost)
        return BadRequest("Insufficient funds.");

    user.ChequingAccount.Balance -= cost;

    // Update or add holding
    var holding = portfolio.Holdings.FirstOrDefault(h => h.StockId == stock.StockId);
    if (holding == null)
    {
        portfolio.Holdings.Add(new PortfolioHolding
        {
            StockId = stock.StockId,
            Quantity = quantity,
            AvgPrice = stock.Price
        });
    }
    else
    {
        holding.AvgPrice = (holding.AvgPrice * holding.Quantity + stock.Price * quantity) / (holding.Quantity + quantity);
        holding.Quantity += quantity;
    }

    await _context.SaveChangesAsync();
    return RedirectToAction("Index", new { userId });
        }
        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken] 
        public async Task<IActionResult> BuyStock(int stockId, int portfolioId, decimal quantity)
        {
            // Authentication Check (Get UserId from Claims)
            var userId = GetCurrentUserId();
            if (userId == null)
            {
                return RedirectToAction("Login", "Authentication");
            }

            // Load User with all required navigation properties
            var user = await _context.Users
                .Include(u => u.ChequingAccount)
                .Include(u => u.Portfolios.Where(p => p.PortfolioId == portfolioId)) // Load only the target portfolio
                    .ThenInclude(p => p.Holdings)
                .FirstOrDefaultAsync(u => u.UserId == userId.Value);

            if (user == null || !user.Portfolios.Any())
                return NotFound("User or Portfolio not found.");

            var portfolio = user.Portfolios.First();

            if (quantity <= 0)
                return BadRequest("Quantity must be greater than zero.");

            // Load Stock from DB and Update Price (API Call)
            var stock = await _context.Stocks.FirstOrDefaultAsync(s => s.StockId == stockId);

            if (stock == null)
                return BadRequest("Stock not found in the database.");

            // Fetch current price from Finnhub (1 API Call per Buy operation is okay)
            var quote = await _stockService.GetStockAsync(stock.Symbol);

            // Update the stock entity with the latest price
            stock.Price = quote.Price;
            stock.LastUpdated = System.DateTime.Now;
            await _context.SaveChangesAsync();

           
            var cost = stock.Price * quantity;
            if (user.ChequingAccount.Balance < cost)
                return BadRequest("Insufficient funds in Chequing Account.");

            user.ChequingAccount.Balance -= cost;

            
            var holding = portfolio.Holdings.FirstOrDefault(h => h.StockId == stock.StockId);

            if (holding == null)
            {
               
                portfolio.Holdings.Add(new PortfolioHolding
                {
                    StockId = stock.StockId,
                    Quantity = quantity,
                    AvgPrice = stock.Price // Initial AvgPrice is the current price
                });
            }
            else
            {
                // Update existing holding (Calculate new weighted average price)
                holding.AvgPrice = (holding.AvgPrice * holding.Quantity + stock.Price * quantity) / (holding.Quantity + quantity);
                holding.Quantity += quantity;
            }

           
            await _context.SaveChangesAsync();

           
            return RedirectToAction("Index");
        }
       

        // SELL STOCK FROM PORTFOLIO INDEX PAGE
        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken] 
        public async Task<IActionResult> SellStock(int portfolioId, int stockId, decimal quantity)
        {
            //Authentication Check (Get UserId from Claims)
            var userId = GetCurrentUserId();
            if (userId == null)
            {
                return RedirectToAction("Login", "Authentication");
            }

            // Load User, Account, and Target Portfolio/Holding
            
            var user = await _context.Users
                .Include(u => u.ChequingAccount)
                .Include(u => u.Portfolios.Where(p => p.PortfolioId == portfolioId))
                    .ThenInclude(p => p.Holdings.Where(h => h.StockId == stockId)) // Load only the target holding
                .FirstOrDefaultAsync(u => u.UserId == userId.Value);

            if (user == null || !user.Portfolios.Any())
                return NotFound("User or Portfolio not found.");

            var portfolio = user.Portfolios.First();
            var holding = portfolio.Holdings.FirstOrDefault(); 

            if (holding == null)
                return BadRequest("Holding not found in portfolio.");

            if (quantity <= 0)
                return BadRequest("Quantity must be greater than zero.");

            if (holding.Quantity < quantity)
                return BadRequest($"Insufficient quantity to sell. Max available: {holding.Quantity}.");

            //Load Stock and Update Price (API Call)
            var stock = await _context.Stocks.FirstOrDefaultAsync(s => s.StockId == stockId);

            if (stock == null)
                return BadRequest("Stock not found in the database.");

            // Fetch current price from Finnhub for sale price
            var quote = await _stockService.GetStockAsync(stock.Symbol);

            // Update the stock entity with the latest price
            stock.Price = quote.Price;
            stock.LastUpdated = System.DateTime.Now;
            await _context.SaveChangesAsync();

            
            var proceeds = stock.Price * quantity;

         
            user.ChequingAccount.Balance += proceeds;

            //  Update or Remove
            holding.Quantity -= quantity;

            if (holding.Quantity == 0)
            {
                // Remove holding if quantity reaches zero
                portfolio.Holdings.Remove(holding);
                _context.PortfolioHoldings.Remove(holding);
            }
            

           
            await _context.SaveChangesAsync();

            
            return RedirectToAction("Index");
        }
    }

}
