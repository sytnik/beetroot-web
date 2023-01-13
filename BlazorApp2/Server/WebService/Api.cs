namespace BlazorApp2.Server.WebService;

public static class Api
{
    internal static void SetMappings(this WebApplication application)
    {
        application.MapGet("department", async (ICompanyRepository company) =>
            JsonSerializer.Serialize(await company.GetDepartmentAsync(), SerializerOptions));
        application.MapGet("users", async (ICompanyRepository company) =>
            await company.GetUsersAsync());
        application.MapPut("createuser", async (UserModel user, ICompanyRepository company) =>
            await company.CreateUserAsync(user) ? Results.Ok(user) : Results.BadRequest(user));
        application.MapGet("readuser", async (int id, ICompanyRepository company) =>
        {
            var user = await company.ReadUserAsync(id);
            return user is not null ? Results.Ok(user) : Results.NotFound(id);
        });
        application.MapPatch("updateuser", async (UserModel user, ICompanyRepository company) =>
            Results.Created($"https://localhost/readuser?id={user.Id}", await company.UpdateUserAsync(user)));
        application.MapDelete("deleteuser", async (int id, ICompanyRepository company) =>
            Results.Ok(await company.DeleteUserAsync(id)));
    }
}