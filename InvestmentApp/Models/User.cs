namespace InvestmentApp.Models
{
    public class User
    {
        public int UserId { get; set; }

        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }

        public string Password { get; set; }

        // Navigation
        public ChequingAccount ChequingAccount { get; set; }
        public List<Portfolio> Portfolios { get; set; }
    }
}
