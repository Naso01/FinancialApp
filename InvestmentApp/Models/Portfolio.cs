using System.ComponentModel.DataAnnotations;

namespace InvestmentApp.Models
{
    /***************************************************************************************
     * Author: Hanjia Li
     * Description:
     *     Represents an investment portfolio owned by a user within the InvestmentApp
     *     system. Each portfolio tracks:
     *       - Its name
     *       - Whether it is Managed or Self-Directed
     *       - The user who owns it
     *       - The collection of holdings (individual stocks + quantities)
     *       - The date the portfolio was created
     *
     *     The Portfolio model is tightly linked to PortfolioHolding entries, allowing the
     *     system to display and calculate investment performance for each user.
     ***************************************************************************************/

    public enum PortfolioType
    {
        Managed = 1,
        SelfDirected = 2
    }

    public class Portfolio
    {
        // Primary key for the portfolio
        public int PortfolioId { get; set; }

        // Custom name chosen by the user (e.g., "Tech Growth", "Retirement Fund")
        public string Name { get; set; }

        // Foreign key reference to the owning user
        public int UserId { get; set; }

        // Indicates whether the portfolio is Managed or Self-Directed
        public PortfolioType PortfolioType { get; set; }

        // Timestamp of when the portfolio was created
        public DateTime CreatedAt { get; set; }

        // Navigation property to the owning user
        public User User { get; set; }

        // Collection of stock holdings within the portfolio
        public List<PortfolioHolding> Holdings { get; set; }
    }
}
