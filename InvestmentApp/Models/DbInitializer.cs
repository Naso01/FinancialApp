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
                    ChequingAccount = new ChequingAccount { Balance = 7500m } }
            };

            foreach (var user in users)
            {
                // creating empty portfolio for each user
                var portfolio = new Portfolio
                {
                    PortfolioType = PortfolioType.SelfDirected,
                    CreatedAt = DateTime.Now,
                    User = user,
                    Holdings = new System.Collections.Generic.List<PortfolioHolding>()
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
