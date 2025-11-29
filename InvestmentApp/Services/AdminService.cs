using InvestmentApp.Models;

namespace InvestmentApp.Services
{
    /***************************************************************************************
     * Author: Hanjia Li
     * Description:
     *     Provides administrative functionality for the InvestmentApp system. This service
     *     handles:
     *       - Validating admin credentials (using a predefined in-memory list)
     *       - Retrieving all users from the database for administrative viewing
     *
     *     Admins are not stored in the database; instead, a hard-coded list is used to
     *     simplify authentication for assignment/demo purposes.
     ***************************************************************************************/

    public class AdminService
    {
        // Hard-coded admin accounts for simplified authentication
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

        // Validates admin login based on the hard-coded admin list
        public Admin? ValidateLogin(string username, string password)
        {
            return _admins.FirstOrDefault(a => a.Username == username && a.Password == password);
        }

        // Retrieves all users from the database with essential information included
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
