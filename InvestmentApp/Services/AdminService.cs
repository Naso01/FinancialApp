using InvestmentApp.Models;

namespace InvestmentApp.Services
{
    public class AdminService
    {
        // Hard-coded admins (no database needed)
        private readonly List<Admin> _admins = new List<Admin>
        {
            new Admin { Username = "Nathan", Password = "NathanAdmin" },
            new Admin { Username = "Keegan", Password = "KeeganAdmin" },
            new Admin { Username = "Hanjia", Password = "HanjiaAdmin" }
        };

        private readonly ApplicationDbContext _db;

        public AdminService(ApplicationDbContext db)
        {
            _db = db;
        }

        public Admin? ValidateLogin(string username, string password)
        {
            return _admins.FirstOrDefault(a => a.Username == username && a.Password == password);
        }

        public List<User> GetAllUsers()
        {
            return _db.Users
                .Select(u => new User
                {
                    UserId = u.UserId,
                    FirstName = u.FirstName,
                    LastName = u.LastName,
                    Email = u.Email,
                    ChequingAccount = u.ChequingAccount,
                    Portfolios = u.Portfolios
                })
                .ToList();
        }
    }
}
