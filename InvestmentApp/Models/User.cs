/***************************************************************************************
* Author: Nathan Serrano
* Description:
*     This model represents an application user within the InvestmentApp system.
*     It stores the user’s personal information, login credentials, and their
*     related financial entities such as chequing accounts and portfolios.
*     This class is part of the MVC "Model" layer and maps directly to the database.
***************************************************************************************/
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
