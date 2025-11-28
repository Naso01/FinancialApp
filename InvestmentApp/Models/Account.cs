namespace InvestmentApp.Models
{
    public enum AccountType
    {
        Chequing = 0,
        Savings = 1
    }

    public class Account
    {
        public int AccountId { get; set; }
        public AccountType AccountType { get; set; }
        public int? ChequingAccountId { get; set; }
        public ChequingAccount ChequingAccount { get; set; }
        public int? SavingsAccountId { get; set; }
        public SavingsAccount SavingsAccount { get; set; }
    }
}
