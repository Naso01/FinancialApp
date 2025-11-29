namespace InvestmentApp.Models
{
    /***************************************************************************************
     * Author: Hanjia Li
     * Description:
     *     Represents an administrative user within the InvestmentApp system. Admin accounts
     *     are used exclusively for system-level management tasks and are authenticated via
     *     the custom AdminService rather than ASP.NET Identity.
     *
     *     Note: Passwords are currently stored as plain text for simplicity. This should be
     *     updated to a secure hashing approach (e.g., SHA256, BCrypt, or ASP.NET Identity)
     *     in future iterations of the project.
     ***************************************************************************************/

    public class Admin
    {
        // Primary key for the admin record
        public int AdminId { get; set; }

        // Username used for admin login
        public string Username { get; set; }

        // Plain-text password (to be replaced with hashed storage)
        public string Password { get; set; }
    }
}
