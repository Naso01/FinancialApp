/***************************************************************************************
 * Author: Keegan Erdis
 * Description:
 *     Provides database seeding functionality for the InvestmentApp system. The
 *     DbInitializer ensures that the database is created and pre-populated with
 *     essential data for testing or initial deployment, including:
 *
 *       • Sample users with initial ChequingAccount balances
 *       • Automatically generated Self-Directed portfolios for each user
 *       • Sample stock entries with symbols and company names
 *
 *     This class helps set up a consistent starting state for development or demo
 *     environments, allowing the system to operate immediately without requiring
 *     manual database entry. It is typically called during application startup.
 ***************************************************************************************/

namespace InvestmentApp.Models
{
    public class DbInitializer
    {
        public static void Initialize(ApplicationDbContext context)
        {
            
            context.Database.EnsureCreated();

            if (context.Users.Any())
            {
                return; 
            }

            // seeding 3 users
            var users = new User[]
            {
                new User { FirstName = "Alice", LastName = "Smith", Email = "alice@mail.com", Password = "alice",
                    ChequingAccount = new ChequingAccount { Balance = 10000m } },
                new User { FirstName = "Bob", LastName = "Johnson", Email = "bob@mail.com", Password = "bob",
                    ChequingAccount = new ChequingAccount { Balance = 5000m } },
                new User { FirstName = "Charlie", LastName = "Brown", Email = "charlie@mail.com", Password = "charlie",
                    ChequingAccount = new ChequingAccount { Balance = 75000m } }
            };

            foreach (var user in users)
            {
                // creating empty portfolio for each user
                var portfolio = new Portfolio
                {
                    Name = $"{user.FirstName}'s Portfolio",
                    PortfolioType = PortfolioType.SelfDirected,
                    CreatedAt = DateTime.Now,
                    User = user,
                    Holdings = new List<PortfolioHolding>()
                };


                user.Portfolios = new System.Collections.Generic.List<Portfolio> { portfolio };

                context.Users.Add(user);
            }

            context.SaveChanges();

            if (!context.Stocks.Any())
            {
                var stocks = new[]
                {
                    new Stock { Symbol = "AAPL", CompanyName = "Apple Inc.", LastUpdated = System.DateTime.Now },
                    new Stock { Symbol = "MSFT", CompanyName = "Microsoft Corp.", LastUpdated = System.DateTime.Now },
                    new Stock { Symbol = "GOOGL", CompanyName = "Alphabet Inc.", LastUpdated = System.DateTime.Now },
                };

                context.Stocks.AddRange(stocks);
                context.SaveChanges();
            }
        }
    }
}
