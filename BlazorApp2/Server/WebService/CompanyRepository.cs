namespace BlazorApp2.Server.WebService;

public class CompanyRepository : ICompanyRepository
{
    private readonly IDbContextFactory<NewDbContext> _factory;
    public CompanyRepository(IDbContextFactory<NewDbContext> factory) => _factory = factory;

    public async Task<Department> GetDepartmentAsync() =>
        await (await _factory.Set<Department>()).Include(department => department.Users)
            .ThenInclude(user => user.Details).FirstAsync();

    public async Task<List<User>> GetUsersAsync() => await (await _factory.Set<User>()).ToListAsync();

    public async Task<bool> CreateUserAsync(UserModel userModel)
    {
        var newUser = await (await _factory.Set<User>())
            .AllAsync(user => user.Id != userModel.Id)
            ? new User(userModel)
            : null;
        return await ValidateAndProcess(newUser, async () => await (await _factory.Set<User>()).AddAsync(newUser!));
    }

    public async Task<User> ReadUserAsync(int id) =>
        await (await _factory.Set<User>()).FirstOrDefaultAsync(user => user.Id == id);

    public async Task<bool> UpdateUserAsync(UserModel userModel)
    {
        var existingUser = await (await _factory.Set<User>())
            .FirstOrDefaultAsync(user => user.Id == userModel.Id);
        return await ValidateAndProcess(existingUser,
            async () => (await _factory.CreateDbContextAsync()).Entry(existingUser).CurrentValues
                .SetValues(new User(userModel)));
    }

    public async Task<bool> DeleteUserAsync(int id)
    {
        var existingUser = await (await _factory.Set<User>())
            .FirstOrDefaultAsync(user => user.Id == id);
        return await ValidateAndProcess(existingUser,
            () => _factory.CreateDbContext().Users.Remove(existingUser));
    }

    private async Task<bool> ValidateAndProcess(User user, Delegate method)
    {
        if (user is null) return false;
        method.DynamicInvoke();
        await _factory.CreateDbContext().SaveChangesAsync();
        return true;
    }
}