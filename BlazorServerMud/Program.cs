using AutoMapper;
using BlazorServerMud.DAO;
using BlazorServerMud.Logic;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.CookiePolicy;
using Microsoft.AspNetCore.Mvc;
using MudBlazor;
using MudBlazor.Services;

var builder = WebApplication.CreateBuilder(args);
builder.Services.Configure<CookiePolicyOptions>(options =>
{
    options.Secure = CookieSecurePolicy.Always;
    options.MinimumSameSitePolicy = SameSiteMode.Lax;
    options.HttpOnly = HttpOnlyPolicy.Always;
});
builder.Services.AddDbContext<NewDbContext>(options => options.UseSqlServer(ConnectionString, contextOptionsBuilder =>
        contextOptionsBuilder.UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery))
    .ConfigureWarnings(warnings => warnings.Ignore(CoreEventId.FirstWithoutOrderByAndFilterWarning)));
builder.Services.AddAuthentication(options => options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie();
builder.Logging.SetMinimumLevel(LogLevel.Warning);
builder.Services.AddMudServices(configuration =>
    configuration.SnackbarConfiguration.PositionClass = Defaults.Classes.Position.BottomRight);
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor()
    .AddCircuitOptions(options => options.DetailedErrors = true)
    .AddHubOptions(options => options.MaximumReceiveMessageSize = 102400000);
var mapper = new MapperConfiguration(expression => expression.AddProfile<AppProfile>()).CreateMapper();
mapper.ConfigurationProvider.AssertConfigurationIsValid();
builder.Services.AddSingleton(mapper);
builder.Services.AddScoped<AppRepository>();
builder.WebHost.UseKestrel(options => options.AddServerHeader = false);
var application = builder.Build();
// application.UseExceptionHandler("/Error");
application.UseHsts().UseHttpsRedirection();
application.UseStaticFiles();
application.UseCookiePolicy().UseAuthentication().UseAuthorization();
application.MapBlazorHub();
application.MapFallbackToPage("/_Host");
MapAuthentication(application);
await application.RunAsync();

void MapAuthentication(IEndpointRouteBuilder webApplication)
{
    webApplication.MapPost("loginhandler",
        async ([FromBody] AuthModel auth, HttpContext context, AppRepository repository) =>
        {
            await repository.SignIn(auth, context);
            // context.Response.Redirect("/");
        });
    // webApplication.MapGet("loginhandler",
    //     async (HttpContext context) =>
    //     {
    //         context.Response.Redirect("/");
    //     });
    webApplication.MapGet("logout", async context => await AppRepository.SignOut(context)).RequireAuthorization();
}