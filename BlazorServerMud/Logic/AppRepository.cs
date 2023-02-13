using System.Security.Claims;
using AutoMapper;
using BlazorServerMud.DAO;
using EFCore.BulkExtensions;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Components.Authorization;

namespace BlazorServerMud.Logic;

public class AppRepository
{
    private readonly NewDbContext _dbContext;
    private readonly IMapper _mapper;

    public AppRepository(NewDbContext dbContext, IMapper mapper)
    {
        _dbContext = dbContext;
        _mapper = mapper;
    }

    public async Task<DepartmentDto> GetDepartmentWithUsers() =>
        await _mapper.ProjectTo<DepartmentDto>(_dbContext.Department).FirstAsync();

    public async Task SubmitPurchase(Purchase purchase)
    {
        await _dbContext.BulkInsertAsync(new List<Purchase> {purchase});
        await _dbContext.BulkInsertAsync(purchase.PurchaseProducts);
    }

    public async Task<List<Product>> GetAllProducts() => await _dbContext.Product.ToListAsync();

    public async Task DeletePurchase(int purchaseId)
    {
        await _dbContext.BulkDeleteAsync(new List<Purchase>
            {await _dbContext.Purchase.FirstAsync(purchase => purchase.Id == purchaseId)});
        await _dbContext.BulkDeleteAsync(await _dbContext.PurchaseProduct
            .Where(purchaseProduct => purchaseProduct.PurchaseId == purchaseId).ToListAsync());
    }

    public async Task<Department> GetDepartmentWithCounter()
    {
        var department = await _dbContext.Department.FirstAsync();
        return department with
        {
            UserCounter =
            await _dbContext.User.CountAsync(user => user.DepartmentId == department.Id)
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

    public async Task<int> GetNewPurchaseId() => await _dbContext.Purchase.MaxAsync(purchase => purchase.Id) + 1;

    public async Task<Manager> GetManager(AuthenticationStateProvider authentication)
    {
        var login = (await authentication.GetAuthenticationStateAsync()).User.Identity?.Name ?? "";
        return await _dbContext.Set<Manager>()
            .Where(manager => manager.Login == login).FirstOrDefaultAsync();
    }

    public async Task<Manager> GetManagerWithData(int id) =>
        await _dbContext.Manager.Include(manager => manager.Purchases)
            .ThenInclude(purchase => purchase.PurchaseProducts)
            .ThenInclude(product => product.Product).FirstAsync(manager => manager.Id == id);

    public async Task SignIn(AuthModel auth, HttpContext context)
    {
        var user = await _dbContext.Manager
            .FirstOrDefaultAsync(manager => manager.Login == auth.Login && manager.Password == auth.Password);
        if (user is null) return;
        await context.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme,
            new ClaimsPrincipal(new ClaimsIdentity(
                new List<Claim>
                {
                    new(ClaimsIdentity.DefaultNameClaimType, user.Login),
                    new(ClaimsIdentity.DefaultRoleClaimType, user.Role)
                },
                "applicationCookie",
                ClaimsIdentity.DefaultNameClaimType, ClaimsIdentity.DefaultRoleClaimType))
        );
        // context.Response.Redirect("/");
    }

    public static async Task SignOut(HttpContext context)
    {
        await context.SignOutAsync();
        context.Response.Redirect("/");
    }
}