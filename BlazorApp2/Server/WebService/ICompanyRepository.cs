namespace BlazorApp2.Server.WebService;

public interface ICompanyRepository
{
    Task<Department> GetDepartmentAsync();
    Task<List<User>> GetUsersAsync();
    Task<bool> CreateUserAsync(UserModel userModel);
    Task<User> ReadUserAsync(int id);
    Task<bool> UpdateUserAsync(UserModel userModel);
    Task<bool> DeleteUserAsync(int id);
}