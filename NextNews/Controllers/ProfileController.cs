using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NextNews.Data;
using NextNews.Models.Database;
using System;

namespace NextNews.Controllers
{
    [Route("profile")]
    public class ProfileController : Controller
    {

        private readonly ApplicationDbContext _DbContext;

        public ProfileController(ApplicationDbContext DbContext)
        {
            _DbContext = DbContext;
        }




        [HttpGet]
        public IActionResult Index()
        {
            // Retrieve the user from the database based on authentication
            var user = _DbContext.Users.FirstOrDefault(u => u.UserName == "current_user_username"); // Replace with actual username retrieval

            if (user == null)
            {
                // Handle case when user is not found
                return NotFound();
            }

            return View(user);
        }


        



        [HttpPost]
        public IActionResult Update(User updatedUser)
        {
            // Retrieve the existing user from the database
            var existingUser = _DbContext.Users.FirstOrDefault(u => u.UserName == "current_user_username"); // Replace with actual username retrieval

            if (existingUser == null)
            {
                // Handle case when user is not found
                return NotFound();
            }

            // Update the user properties

            existingUser.DateofBirth = updatedUser.DateofBirth;
            existingUser.Email = updatedUser.Email;
            //existingUser.PhoneNumber = updatedUser.PhoneNumber;
            existingUser.FirstName = updatedUser.FirstName;
            existingUser.LastName = updatedUser.LastName;
            existingUser.UserName = updatedUser.UserName;

            //Save changes to the database
            _DbContext.SaveChanges();

            return RedirectToAction("Index");
        }
    }


}

