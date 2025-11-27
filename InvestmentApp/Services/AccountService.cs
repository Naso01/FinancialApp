using InvestmentApp.Models;

namespace InvestmentApp.Services
{
    public class AccountService
    {
        private readonly ApplicationDbContext _db;

        public AccountService(ApplicationDbContext db)
        {
            _db = db;
        }

        public bool EditAccount(int accountId /* + model later */)
        {
            // logic later
            return true;
        }

        public bool DeleteAccount(int accountId)
        {
            // logic later
            return true;
        }
    }
}