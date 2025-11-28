using InvestmentApp.Models;

namespace InvestmentApp.Services
{
    public class PortfolioService
    {
        private readonly ApplicationDbContext _db;

        public PortfolioService(ApplicationDbContext db)
        {
            _db = db;
        }

        public bool AddHolding(int portfolioId /* + model later */)
        {
            // logic later
            return true;
        }

        public bool EditHolding(int holdingId)
        {
            // logic later
            return true;
        }

        public bool DeleteHolding(int holdingId)
        {
            // logic later
            return true;
        }
    }
}