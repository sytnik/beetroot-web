namespace BlazorApp2.Server.Logic;

public static class Extensions
{
    public static async Task<DbSet<T>> Set<T>(this IDbContextFactory<NewDbContext> factory) where T : class =>
        (await factory.CreateDbContextAsync()).Set<T>();
}