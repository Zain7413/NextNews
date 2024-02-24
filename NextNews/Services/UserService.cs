using Azure.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.EntityFrameworkCore;
using NextNews.Data;
using NextNews.Models.Database;

namespace NextNews.Services
{
    public class UserService : IUserService
    {

        private readonly ApplicationDbContext _context;
        private readonly UserManager<User> _userManager;



        public UserService(ApplicationDbContext context)
        {
            _context = context;
          
        }

       

        public List<User> GetUsers()
        {
            var users = _context.Users.ToList();
            return users;
            //return _context.Users.ToList();
        }


        public async Task<List<User>> GetUsersAsync()
        {
            return await _context.Users.ToListAsync();
        }


        public async Task<User> GetUserByIdAsync(string id)
        {
            //var usr = await _userManager.FindByIdAsync(id);

            return await _context.Users.FindAsync(id);

        }


        public User GetUserById(string userId)
        {
            User usr = _context.Users.FirstOrDefault(u => u.Id == userId);
            return usr;
        }



        public async Task<bool> CreateUserAsync(User user)
        {
            try
            {
                // Map User to your data model (assuming User is your data model)
                var userModel = new User
                {
                    UserName = user.UserName,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    DateofBirth = user.DateofBirth,
                    Email = user.Email,
                    PhoneNumber = user.PhoneNumber,
                };
         
                _context.Users.Add(userModel);

                // Save changes 
                var result = await _context.SaveChangesAsync();

                // Checking if at least one row was affected
                return result > 0;
            }
            catch (Exception ex)
            {
                return false;
            }
        }


        public async Task<bool> UpdateUserAsync(User user)
        {
            try
            {
                var existingUser = await _userManager.FindByIdAsync(user.Id);

                if (existingUser == null)
                {
                    // User not found
                    return false;
                }

                // Update user properties
                existingUser.FirstName = user.FirstName;
                existingUser.LastName = user.LastName;
                existingUser.DateofBirth = user.DateofBirth;
                existingUser.PhoneNumber= user.PhoneNumber;
                existingUser.Email=user.Email;
              

                // Save changes using UserManager
                var result = await _userManager.UpdateAsync(existingUser);

                return result.Succeeded;
            }
            catch (Exception ex)
            {
                
                return false;
            }
        }



        public async Task DeleteUserAsync(string id)
        {
            try
            {
                var users = await _context.Users.FindAsync(id);

                if (users != null)
                {
                    _context.Users.Remove(users);
                    await _context.SaveChangesAsync();
                }


            }

            catch (Exception ex)
            {
                
                throw;


            }


        }

        public void GetUserForLike() 
        {
            _context.Users.ToList();
        }


        public List<Subscription> GetUsersSubscriptions(string userId)
        {
           var objList = _context.Subscriptions.Where(s => s.UserId == userId).OrderByDescending(s => s.Created).ToList();

            return objList;
        }


    }
}
