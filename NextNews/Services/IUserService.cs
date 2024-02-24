using Microsoft.AspNetCore.Mvc;
using NextNews.Models.Database;
using NextNews.ViewModels;

namespace NextNews.Services
{
    public interface IUserService
    {

        List<User> GetUsers();

        Task<List<User>> GetUsersAsync();

        Task<User> GetUserByIdAsync(string id);

        User GetUserById(string userId);

        Task<bool> CreateUserAsync(User user);

        Task<bool> UpdateUserAsync(User user);

       
        //Task GetUserByIdAsync(string id);

        Task DeleteUserAsync(string id);


        


        List<Subscription> GetUsersSubscriptions(string userId);







    }
}
