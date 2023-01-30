using Microsoft.AspNetCore.Mvc;

namespace BlazorApp2.Server.WebService;

public static class Api
{
    internal static void SetMappings(this WebApplication application)
    {
        application.MapGet("department", async (CompanyRepository company) =>
            JsonSerializer.Serialize(await company.GetDepartmentAsync(), SerializerOptions));
        application.MapGet("users", async (CompanyRepository company) =>
            JsonSerializer.Serialize(await company.GetUsersAsync(), SerializerOptions));
        application.MapPut("createuser", async (UserModel user, CompanyRepository company) =>
            await company.CreateUserAsync(user) ? Results.Ok(user) : Results.BadRequest(user));
        application.MapGet("readuser", async (int id, CompanyRepository company) =>
        {
            var user = await company.ReadUserAsync(id);
            return user is not null ? Results.Ok(user) : Results.NotFound(id);
        });
        application.MapPatch("updateuser", async (UserModel user, CompanyRepository company) =>
            Results.Created($"https://localhost/readuser?id={user.Id}", await company.UpdateUserAsync(user)));
        application.MapDelete("deleteuser", async (int id, CompanyRepository company) =>
            Results.Ok(await company.DeleteUserAsync(id)));
        application.MapPost("/login",
            async (HttpContext httpContext, AuthModel auth, NewDbContext dbContext) =>
            {
                await httpContext.AuthHandler(auth, dbContext);
                httpContext.Response.Redirect("/");
            });
        application.MapGet("/getmanager", async (HttpContext httpContext, NewDbContext dbContext) =>
            await httpContext.GetManager(dbContext)
        );
        application.MapGet("/getmanagerwithdata", async (int id, HttpContext httpContext, NewDbContext dbContext) =>
        {
            var manager = await httpContext.GetManagerWithData(id, dbContext);
            return JsonSerializer.Serialize(manager, SerializerOptions);
        });
        application.MapGet("/getallproducts", async (HttpContext httpContext, NewDbContext dbContext) =>
            await httpContext.GetAllProducts(dbContext)
        );
        application.MapGet("/GetNewPurchaseId", async (HttpContext httpContext, NewDbContext dbContext) =>
            await httpContext.GetNewPurchaseId(dbContext)
        );

        application.MapPost("SubmitPurchase",
            async (CompanyRepository companyRepository, [FromBody] Purchase purchase) =>
                await companyRepository.SubmitPurchase(purchase));
        application.MapDelete("DeletePurchase",
            (CompanyRepository companyRepository, int id) => companyRepository.DeletePurchase(id));

        application.MapGet("/GetDepartmentWithCounter", async (CompanyRepository company) =>
            JsonSerializer.Serialize(await company.GetDepartmentWithCounter(), SerializerOptions));
        application.MapGet("/logout", async httpContext => await httpContext.SignOutAsync());
    }
}