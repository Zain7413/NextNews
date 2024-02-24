using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NextNews.Models.Database;
using NextNews.Services;
using NextNews.ViewModels;

namespace NextNews.Controllers
{
    public class UserController : Controller
    {
        private readonly IUserService _userService;
        private readonly UserManager<User> _userManager;

        public UserController(IUserService userService, UserManager<User> userManager)
        {
            _userService = userService;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index(string username)
        {
            List<User> users = await _userService.GetUsersAsync();

            //if (!String.IsNullOrEmpty(firstname))          // use for firstname
            //{
            //    users = users.Where(x => x.FirstName.Contains(firstname)).ToList();
            //}


            if (!String.IsNullOrEmpty(username))
            {
                users = users.Where(x => x.UserName.Contains(username)).ToList();
            }

            return View(users);
        }


        [Authorize(Roles = "Admin, Editor")]
        public IActionResult ManageUsers()
        {
            // Getting a list of users from the service
            var users = _userService.GetUsers();

            return View(users);
        }

        public async Task<IActionResult> Details(string id)
        {
            var user = await _userService.GetUserByIdAsync(id);

            if (user == null)
            {
                return NotFound();
            }


            return View(user);
        }



        [HttpGet]
        public async Task<IActionResult> Edit(string id)
        {
            var existingUser = await _userService.GetUserByIdAsync(id);

            if (existingUser == null)
            {
                // User not found
                return NotFound();
            }

            return View(existingUser);

        }


        [HttpPost]
        public async Task<IActionResult> EditConfirmed(User user)
        {
            if (user == null)
            {
                return BadRequest();
            }
            if (!ModelState.IsValid)
            {
                // Model state is not valid, return the view with validation errors
                return View("Edit", user);
            }
            await _userService.UpdateUserAsync(user);

            return RedirectToAction("ManageUsers");
        }




        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteUser(string id)
        {
            var user = await _userService.GetUserByIdAsync(id);

            if (user == null)
            {
                return NotFound();
            }

            return View(user);
        }


        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            await _userService.DeleteUserAsync(id);
            return RedirectToAction(nameof(ManageUsers));

        }

        [Authorize(Roles = "Admin")]
        public IActionResult DeleteConfirmed()
        {

            return View();
        }



        //Assign role to user by Admin


        [Authorize(Roles = "Admin, Editor")]
        [HttpPost]
        public async Task<IActionResult> AssignRole(string userId, string role)
        {
            var user = await _userService.GetUserByIdAsync(userId);

            if (user == null)
            {
                return NotFound();
            }

            if (!string.IsNullOrEmpty(role))
            {
                // Check if the user is already in the specified role
                var isInRole = await _userManager.IsInRoleAsync(user, role);

                if (!isInRole)
                {
                    // Assign the role to the user
                    await _userManager.AddToRoleAsync(user, role);
                }
            }

            return RedirectToAction("Details", new { id = userId });
        }



  
        public IActionResult UserDashboard()
        {
            return View();
        }



        public IActionResult MyPages()
        {
            string userId = _userManager.GetUserId(HttpContext.User) ?? "";
            User user = _userService.GetUserById(userId);

            var vm = new UserVM { 
            User = user, 
            MySubscriptions = _userService.GetUsersSubscriptions(userId),
            };


            return View(vm);
        }






    }
}
