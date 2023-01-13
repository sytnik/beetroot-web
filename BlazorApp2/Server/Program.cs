var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDbContext<NewDbContext>(options => options.UseSqlServer(ConnectionString))
    .AddScoped<ICompanyRepository, CompanyRepository>()
    .AddEndpointsApiExplorer()
    .AddSwaggerGen();
builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();
var app = builder.Build();
if (app.Environment.IsDevelopment())
{
    app.UseWebAssemblyDebugging();
}
else
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseSwagger()
    .UseSwaggerUI(options=>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "Our test blazor API");
        options.RoutePrefix = "somehiddentest";
    })
    .UseHttpsRedirection();
app.UseBlazorFrameworkFiles();
app.UseStaticFiles();
app.UseRouting();
app.MapRazorPages();
app.MapControllers();
app.MapFallbackToFile("index.html");
app.SetMappings();
await app.RunAsync();
