namespace InvestmentApp.Models
{
    /***************************************************************************************
     * Author: Hanjia Li
     * Description:
     *     Defines the types of accounts available in the InvestmentApp system and the
     *     Account model that links a user to either a ChequingAccount or SavingsAccount.
     *
     *     This acts as a polymorphic wrapper that allows a single "Account" reference
     *     to point to one of two concrete account types. Only one of the two navigation
     *     properties (Chequing or Savings) will be populated at any time.
     ***************************************************************************************/

    public enum AccountType
    {
        Chequing = 0,
        Savings = 1
    }

    public class Account
    {
        // Primary key for the generic account reference
        public int AccountId { get; set; }

        // Indicates whether this account refers to a Chequing or Savings account
        public AccountType AccountType { get; set; }

        // If AccountType == Chequing, this foreign key will be populated
        public int? ChequingAccountId { get; set; }
        public ChequingAccount ChequingAccount { get; set; }

        // If AccountType == Savings, this foreign key will be populated
        public int? SavingsAccountId { get; set; }
        public SavingsAccount SavingsAccount { get; set; }
    }
}
