using InvestmentApp.Models;
using Microsoft.EntityFrameworkCore;

/***************************************************************************************
 * Author: Hanjia Li
 * Description:
 *     Provides all business logic for managing user financial accounts within the
 *     InvestmentApp system. This includes:
 *       - Retrieving chequing or savings accounts
 *       - Creating new chequing or savings accounts
 *       - Editing balances
 *       - Adding funds
 *       - Calculating savings interest
 *       - Deleting accounts and associated type records
 *
 *     This service acts as the logic layer between controllers and the database, ensuring
 *     consistent and centralized handling of account operations.
 ***************************************************************************************/

namespace InvestmentApp.Services
{
    public class AccountService
    {
        private readonly ApplicationDbContext _db;

        public AccountService(ApplicationDbContext db)
        {
            _db = db;
        }

        // Retrieves either a ChequingAccount or SavingsAccount for a user
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

        // Creates a new chequing account for the user (only one allowed)
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

        // Creates a new savings account for the user (only one allowed)
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

        // Returns the user's savings account (if any)
        public SavingsAccount? GetSavingsAccount(int userId)
        {
            return _db.SavingsAccounts
                .Include(s => s.Account)
                .FirstOrDefault(s => s.UserId == userId);
        }

        // Calculates interest for a savings account at a fixed rate
        public decimal CalculateSavingsInterest(decimal balance)
        {
            const decimal rate = 0.0125m;
            return balance * rate;
        }

        // Adds funds to whichever account the user has (chequing or savings)
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

        // Edits the balance of the user's chequing account
        public bool EditAccount(int userId, decimal balance)
        {
            var account = _db.ChequingAccounts.FirstOrDefault(a => a.UserId == userId);
            if (account == null) return false;

            account.Balance = balance;
            _db.SaveChanges();
            return true;
        }

        // Deletes either a chequing or savings account along with its type entry
        public bool DeleteAccount(int userId)
        {
            var chequing = _db.ChequingAccounts.FirstOrDefault(a => a.UserId == userId);
            var savings = _db.SavingsAccounts.FirstOrDefault(a => a.UserId == userId);

            if (chequing == null && savings == null)
                return false;

            if (chequing != null)
            {
                var acc = _db.Accounts.FirstOrDefault(a => a.ChequingAccountId == chequing.ChequingAccountId);
                if (acc != null) _db.Accounts.Remove(acc);

                _db.ChequingAccounts.Remove(chequing);
            }

            if (savings != null)
            {
                var acc = _db.Accounts.FirstOrDefault(a => a.SavingsAccountId == savings.SavingsAccountId);
                if (acc != null) _db.Accounts.Remove(acc);

                _db.SavingsAccounts.Remove(savings);
            }

            _db.SaveChanges();
            return true;
        }
    }
}
