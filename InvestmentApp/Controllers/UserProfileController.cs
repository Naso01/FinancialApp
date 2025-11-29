using InvestmentApp.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

/***************************************************************************************
 * Author: Nathan Serrano
 * Description:
 *     This controller manages all user profile–related functionality within the
 *     InvestmentApp system. It allows authenticated users to view, edit, and delete
 *     their profile information. The controller ensures secure access by requiring
 *     user authentication and retrieves user data through claims-based identity.
 ***************************************************************************************/


namespace InvestmentApp.Controllers
{
    [Authorize]
    public class UserProfileController : Controller
    {
        private readonly ApplicationDbContext _context;

        // Injects the application database context into the controller
        public UserProfileController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Displays the user’s profile information
        public IActionResult Index()
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
            var user = _context.Users.FirstOrDefault(u => u.UserId == userId);
            return View(user);
        }

        // GET: Displays the edit profile form populated with the user's data
        public IActionResult Edit()
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
            var user = _context.Users.First(u => u.UserId == userId);
            return View(user);
        }

        // POST: Saves the edited profile information back to the database
        [HttpPost]
        public IActionResult Edit(User updated)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
            var user = _context.Users.First(u => u.UserId == userId);

            user.FirstName = updated.FirstName;
            user.LastName = updated.LastName;
            user.Email = updated.Email;

            if (!string.IsNullOrEmpty(updated.Password))
                user.Password = updated.Password;

            _context.SaveChanges();

            return RedirectToAction("Index");
        }

        // GET: Displays a confirmation screen for account deletion
        public IActionResult Delete()
        {
            return View();
        }

        // POST: Permanently deletes the user's account from the database
        [HttpPost]
        public IActionResult DeleteConfirmed()
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
            var user = _context.Users.First(u => u.UserId == userId);

            _context.Users.Remove(user);
            _context.SaveChanges();

            return RedirectToAction("Logout", "Authentication");
        }
    }
}
