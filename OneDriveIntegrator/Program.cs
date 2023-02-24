using OneDriveIntegrator.Extensions.DependencyInjection;

var builder = WebApplication.CreateBuilder(args);
var configuration = builder.Configuration;
var environment = builder.Environment;

builder.Services
    .AddHttpContextAccessor()
    .AddHttpClient(configuration)
    .AddGraphClient()
    .AddAuthenticationService(configuration, environment)
    .AddTokensStorage(configuration)
    .AddSubscription(configuration)
    .AddControllersWithViews();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
app.UseAuthenticationService();
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
app.Run();