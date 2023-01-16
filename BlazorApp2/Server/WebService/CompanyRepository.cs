namespace BlazorApp2.Server.WebService;

public class CompanyRepository : ICompanyRepository
{
    private readonly NewDbContext _dbContext;
    public CompanyRepository(NewDbContext dbContext) => _dbContext = dbContext;

    public async Task<Department> GetDepartmentAsync() =>
        await _dbContext.Set<Department>().Include(department => department.Users)
            .ThenInclude(user => user.Details).FirstAsync();

    public async Task<List<User>> GetUsersAsync() => await _dbContext.Set<User>().ToListAsync();

    public async Task<bool> CreateUserAsync(UserModel userModel)
    {
        var newUser = await _dbContext.Set<User>().AllAsync(user => user.Id != userModel.Id)
            ? new User(userModel)
            : null;
        return await ValidateAndProcess(newUser, async () => await _dbContext.Set<User>().AddAsync(newUser!));
    }

    public async Task<User> ReadUserAsync(int id) =>
        await _dbContext.Set<User>().FirstOrDefaultAsync(user => user.Id == id);

    public async Task<bool> UpdateUserAsync(UserModel userModel)
    {
        var existingUser = await _dbContext.Set<User>()
            .FirstOrDefaultAsync(user => user.Id == userModel.Id);
        return await ValidateAndProcess(existingUser,
            () => _dbContext.Entry(existingUser).CurrentValues
                .SetValues(new User(userModel)));
    }

    public async Task<bool> DeleteUserAsync(int id)
    {
        var existingUser = await _dbContext.Set<User>()
            .FirstOrDefaultAsync(user => user.Id == id);
        return await ValidateAndProcess(existingUser, () => _dbContext.Set<User>().Remove(existingUser));
    }

    private async Task<bool> ValidateAndProcess(User user, Delegate method)
    {
        if (user is null) return false;
        method.DynamicInvoke();
        await _dbContext.SaveChangesAsync();
        return true;
    }
}