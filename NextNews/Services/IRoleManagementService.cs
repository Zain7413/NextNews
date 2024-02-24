namespace NextNews.Services
{
    public interface IRoleManagementService
    {

        Task<bool> CreateRoleAsync(string roleName);


    }
}
