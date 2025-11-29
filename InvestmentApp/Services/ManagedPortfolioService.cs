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
