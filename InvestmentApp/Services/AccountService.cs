using InvestmentApp.Models;
using Microsoft.EntityFrameworkCore;

namespace InvestmentApp.Services
{
    public class AccountService
    {
        private readonly ApplicationDbContext _db;

        public AccountService(ApplicationDbContext db)
        {
            _db = db;
        }

        //returns either ChequingAccount or SavingsAccount object, or null if none found
        public object? GetAccount(int userId)
        {
        
            var cheq = _db.ChequingAccounts
                .Include(a => a.Account)
                .FirstOrDefault(a => a.UserId == userId);

            if (cheq != null)
                return cheq;

            var save = _db.SavingsAccounts
                .Include(a => a.Account)
                .FirstOrDefault(a => a.UserId == userId);

            return save;
        }


        public bool AddChequingAccount(int userId, decimal startingBalance)
        {
            if (_db.ChequingAccounts.Any(a => a.UserId == userId))
                return false;

            var account = new ChequingAccount
            {
                UserId = userId,
                Balance = startingBalance
            };

            _db.ChequingAccounts.Add(account);
            _db.SaveChanges();

            var typeRecord = new Account
            {
                AccountType = AccountType.Chequing,
                ChequingAccountId = account.ChequingAccountId
            };

            _db.Accounts.Add(typeRecord);
            _db.SaveChanges();

            return true;
        }

        public bool AddSavingsAccount(int userId, decimal startingBalance)
        {
            if (_db.SavingsAccounts.Any(a => a.UserId == userId))
                return false;

            var account = new SavingsAccount
            {
                UserId = userId,
                Balance = startingBalance
            };

            _db.SavingsAccounts.Add(account);
            _db.SaveChanges();

            var typeRecord = new Account
            {
                AccountType = AccountType.Savings,
                SavingsAccountId = account.SavingsAccountId
            };

            _db.Accounts.Add(typeRecord);
            _db.SaveChanges();

            return true;
        }

        public bool AddFunds(int userId, decimal amount)
        {
            var cheq = _db.ChequingAccounts.FirstOrDefault(a => a.UserId == userId);
            if (cheq != null)
            {
                cheq.Balance += amount;
                _db.SaveChanges();
                return true;
            }

            var save = _db.SavingsAccounts.FirstOrDefault(a => a.UserId == userId);
            if (save != null)
            {
                save.Balance += amount;
                _db.SaveChanges();
                return true;
            }

            return false;
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
            // get the chequing account first
            var chequing = _db.ChequingAccounts.FirstOrDefault(a => a.UserId == userId);
            if (chequing == null)
                return false;

            // delete the Account entry that points to this chequing account
            var accountRecord = _db.Accounts.FirstOrDefault(a => a.ChequingAccountId == chequing.ChequingAccountId);
            if (accountRecord != null)
                _db.Accounts.Remove(accountRecord);

            // now delete the chequing account
            _db.ChequingAccounts.Remove(chequing);

            _db.SaveChanges();
            return true;
        }


    }
}
