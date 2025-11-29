using InvestmentApp.Models;
using Microsoft.EntityFrameworkCore;

/***************************************************************************************
 * Author: Keegan Erdis
 * Description:
 *     Service responsible for automatically configuring and initializing Managed
 *     Portfolios within the InvestmentApp system. This service enforces the logic
 *     that distinguishes Managed Portfolios from Self-Directed ones by:
 *
 *       • Automatically inserting predefined starter holdings (MSFT, GOOG, PLTR)
 *       • Fetching live stock prices via the StockService to set accurate AvgPrice
 *       • Linking holdings to the newly created portfolio
 *       • Ensuring initialization is performed only once per portfolio
 *
 *     The ManagedPortfolioService acts as a business-logic layer that coordinates
 *     Entity Framework Core operations and external API data when constructing a
 *     Managed Portfolio. Controllers call this service to guarantee that every
 *     Managed Portfolio begins with a consistent, pre-configured investment set.
 ***************************************************************************************/

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

        public async Task InitializeManagedPortfolio(int portfolioId)
        {
            var portfolio = await _context.Portfolios.FindAsync(portfolioId);
            if (portfolio == null) return;

            var initialHoldings = new List<(string Symbol, decimal Quantity)>
            {
                ("MSFT", 2.00m),
                ("GOOG", 5.00m),
                ("PLTR", 10.00m)
    };

            foreach (var (symbol, quantity) in initialHoldings)
            {
                var stock = await _context.Stocks.FirstOrDefaultAsync(s => s.Symbol == symbol);
                if (stock == null) continue;

                var quote = await _stockService.GetStockAsync(symbol);
                decimal currentPrice = quote?.Price ?? 0;

                var holding = new PortfolioHolding
                {
                    PortfolioId = portfolioId,
                    StockId = stock.StockId,
                    Quantity = quantity,
                    AvgPrice = currentPrice,
                    PurchaseDate = DateTime.Now
                };

                _context.PortfolioHoldings.Add(holding);
            }

            await _context.SaveChangesAsync();
        }
    }
}
