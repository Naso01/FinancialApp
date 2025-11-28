using Microsoft.EntityFrameworkCore;


namespace InvestmentApp.Models
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options) { }

        public DbSet<User> Users { get; set; }
        public DbSet<ChequingAccount> ChequingAccounts { get; set; }
        public DbSet<Portfolio> Portfolios { get; set; }
        public DbSet<PortfolioHolding> PortfolioHoldings { get; set; }
        public DbSet<Stock> Stocks { get; set; }
        public DbSet<AdminRecommendation> AdminRecommendations { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
           

            // Portfolio holdings decimal precision
            modelBuilder.Entity<PortfolioHolding>()
                .Property(p => p.Quantity)
                .HasPrecision(18, 6);

            modelBuilder.Entity<PortfolioHolding>()
                .Property(p => p.AvgPrice)
                .HasPrecision(18, 4);

            // Chequing account decimal precision
            modelBuilder.Entity<ChequingAccount>()
                .Property(c => c.Balance)
                .HasPrecision(18, 4);

            // Relationships
            modelBuilder.Entity<User>()
                .ToTable("Users")
                .HasOne(u => u.ChequingAccount)
                .WithOne(c => c.User)
                .HasForeignKey<ChequingAccount>(c => c.UserId);

            modelBuilder.Entity<Stock>()
                .HasIndex(s => s.Symbol)
                .IsUnique();

            modelBuilder.Entity<AdminRecommendation>() 
                .HasKey(ar => ar.RecommendationId);
            
            base.OnModelCreating(modelBuilder);
        }
    }
}

