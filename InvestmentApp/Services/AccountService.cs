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

        public ChequingAccount? GetAccount(int userId)
        {
            return _db.ChequingAccounts.FirstOrDefault(a => a.UserId == userId);
        }

        public bool AddChequingAccount(int userId, decimal startingBalance)
        {
            // user already has one? reject.
            if (_db.ChequingAccounts.Any(a => a.UserId == userId))
                return false;

            var account = new ChequingAccount
            {
                UserId = userId,
                Balance = startingBalance
            };

            _db.ChequingAccounts.Add(account);
            _db.SaveChanges();
            return true;
        }
        public bool EditAccount(int userId, decimal balance)
        {
            var account = _db.ChequingAccounts.FirstOrDefault(a => a.UserId == userId);
            if (account == null) return false;
            account.Balance = balance;
            _db.SaveChanges();
            return true;
        }

        public bool DeleteAccount(int userId)
        {
            var account = _db.ChequingAccounts.FirstOrDefault(a => a.UserId == userId);

            if (account == null)
                return false;

            _db.ChequingAccounts.Remove(account);
            _db.SaveChanges();
            return true;
        }

    }
}