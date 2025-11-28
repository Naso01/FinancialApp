using InvestmentApp.Models;
using Microsoft.EntityFrameworkCore;

namespace InvestmentApp.Services
{
    public class ManagedPortfolioService
    {
        private readonly ApplicationDbContext _context;
        private readonly StockService _stockService;

        public ManagedPortfolioService(ApplicationDbContext context, StockService stockService)
        {
            _context = context;
            _stockService = stockService;
        }

        public async Task InitializeManagedPortfolio(int userId, decimal initialDeposit)
        {
            //Create a new Portfolio for the user
            var portfolio = new Portfolio
            {
                UserId = userId,
                PortfolioType = PortfolioType.Managed, 
                CreatedAt = DateTime.Now
            };
            _context.Portfolios.Add(portfolio);
            await _context.SaveChangesAsync();

            // Define the initial holdings (2 shares of MSFT, 5 shares of GOOG)
            // Must already be in Db
            var initialHoldings = new List<(string Symbol, decimal Quantity)>
        {
            ("MSFT", 2.00m),
            ("GOOG", 5.00m),
            ("PLTR", 10.00m)
        };

            foreach (var (symbol, quantity) in initialHoldings)
            {
                var stock = await _context.Stocks.FirstOrDefaultAsync(s => s.Symbol == symbol);

                if (stock != null)
                {
                    // Fetch the current price from the API 
                    var quote = await _stockService.GetStockAsync(symbol);
                    decimal currentPrice = quote?.Price ?? 0; 

                    decimal cost = quantity * currentPrice;

                    //Create the Holding
                    var holding = new PortfolioHolding
                    {
                        PortfolioId = portfolio.PortfolioId,
                        StockId = stock.StockId,
                        Quantity = quantity,
                        // For initialization, AvgPrice is the current price
                        AvgPrice = currentPrice,
                        PurchaseDate = DateTime.Now
                    };
                    _context.PortfolioHoldings.Add(holding);

                    // Subtract cost from the initial deposit
                                        
                    initialDeposit -= cost;
                }
            }

            
            var userAccount = await _context.ChequingAccounts.FirstOrDefaultAsync(a => a.UserId == userId);
            if (userAccount != null)
            {
                userAccount.Balance = initialDeposit;
                _context.ChequingAccounts.Update(userAccount);
            }

            await _context.SaveChangesAsync();
        }
    }
}
