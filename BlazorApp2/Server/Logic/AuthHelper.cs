﻿namespace BlazorApp2.Server.Logic;

public static class AuthHelper
{
    public static async Task AuthHandler(this HttpContext context, AuthModel auth,
        IDbContextFactory<NewDbContext> factory)
    {
        var user = await (await factory.Set<Manager>())
            .Where(manager => manager.Login == auth.Login && manager.Password == auth.Password)
            .FirstOrDefaultAsync();
        if (user is not null)
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
    }

    public static async Task<Manager> GetManager(this HttpContext context, IDbContextFactory<NewDbContext> factory)
    {
        var login = context.User.Identity?.Name ?? "";
        return await (await factory.Set<Manager>())
            .Where(manager => manager.Login == login).FirstOrDefaultAsync();
    }
}