var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDbContext<NewDbContext>(options => options.UseSqlServer(ConnectionString, contextOptionsBuilder =>
        contextOptionsBuilder.UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery))
    .ConfigureWarnings(warnings => warnings.Ignore(CoreEventId.FirstWithoutOrderByAndFilterWarning)));
builder.Services.AddAuthentication(options => options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie();
builder.Services.AddScoped<CompanyRepository>();
builder.Services.AddEndpointsApiExplorer().AddSwaggerGen();
builder.Services.AddRazorPages();
var app = builder.Build();
if (app.Environment.IsDevelopment()) app.UseWebAssemblyDebugging();
app.UseHsts();
app.UseSwagger().UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "Our test blazor API");
        options.RoutePrefix = "somehiddentest";
    })
    .UseHttpsRedirection();
app.UseBlazorFrameworkFiles().UseStaticFiles().UseRouting()
    .UseAuthentication().UseAuthorization();
app.MapRazorPages();
app.MapFallbackToFile("index.html");
app.SetMappings();
await app.RunAsync();