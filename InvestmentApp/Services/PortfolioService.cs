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

        public bool AddHolding(int portfolioId)
        {
            return true;
        }

        public bool EditHolding(int holdingId)
        {
            return true;
        }

        public bool DeleteHolding(int holdingId)
        {
            return true;
        }
    }
}