using EFCore.BulkExtensions;

namespace BlazorApp2.Server.WebService;

public class CompanyRepository
{
    private readonly NewDbContext _dbContext;
    public CompanyRepository(NewDbContext dbContext) => _dbContext = dbContext;

    public async Task SubmitPurchase(Purchase purchase)
    {
        // save the main purchase
        await _dbContext.BulkInsertAsync(new List<Purchase> {purchase});
        // save m2m entities
        await _dbContext.BulkInsertAsync(purchase.PurchaseProducts);
    }

    public async Task DeletePurchase(int purchaseId)
    {
        // delete purchase by id
        await _dbContext.BulkDeleteAsync(new List<Purchase>
            {await _dbContext.Purchase.FirstAsync(purchase => purchase.Id == purchaseId)});
        // delete binding entities with this id
        await _dbContext.BulkDeleteAsync(await _dbContext.PurchaseProduct
            .Where(purchaseProduct => purchaseProduct.PurchaseId == purchaseId).ToListAsync());
    }

    public async Task<Department> GetDepartmentWithCounter()
    {
        // var department = await _dbContext.Department.Include(d => d.Users).FirstAsync();
        // department.UserCounter = department.Users.Count;
        // department.Users = null;
        // return department;
        var department = await _dbContext.Department.FirstAsync();
        return department with
        {
            UserCounter =
            await _dbContext.Users.CountAsync(user => user.DepartmentId == department.Id)
        };
    }

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