namespace InvestmentApp.Models
{
    /***************************************************************************************
     * Author: Hanjia Li
     * Description:
     *     Represents a user's savings account within the InvestmentApp system. Each
     *     SavingsAccount stores its current balance and is linked to a specific user.
     * 
     *     This model also ties into the generic Account wrapper, allowing the system to
     *     treat Chequing and Savings accounts uniformly while still maintaining separate
     *     table structures for each account type.
     ***************************************************************************************/

    public class SavingsAccount
    {
        // Primary key for the savings account
        public int SavingsAccountId { get; set; }

        // The user who owns this savings account
        public int UserId { get; set; }

        // Current balance of the savings account
        public decimal Balance { get; set; }

        // Navigation property to the generic Account wrapper (optional)
        public Account Account { get; set; }
    }
}
