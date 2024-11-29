using Jobportalwebsite.Data; // Access ApplicationDbContext and NotificationService
using Jobportalwebsite.Models; // Access ApplicationUser
using Microsoft.EntityFrameworkCore; // Required for EF Core
using Microsoft.AspNetCore.Identity; // If using Identity
using Microsoft.Extensions.DependencyInjection; // Required for DI
using System;
using System.Threading.Tasks;
using Jobportalwebsite.Services;
using Jobportalwebsite.IHelper;
using Jobportalwebsite.Helper; // Ensure correct namespace


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddScoped<IUserHelper, UserHelper>();
// Add Razor Pages with runtime compilation
builder.Services.AddRazorPages().AddRazorRuntimeCompilation();

// Configure ApplicationDbContext with SQL Server
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Add Identity services with ApplicationUser
builder.Services.AddDefaultIdentity<ApplicationUser>()
    .AddRoles<IdentityRole>() // Add roles to Identity
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders();

// Add the NotificationService
builder.Services.AddScoped<NotificationService>();

// Seed roles asynchronously at app startup
var app = builder.Build();

// Seed roles at application startup
using (var scope = app.Services.CreateScope())
{
    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
    await SeedRolesAsync(scope.ServiceProvider, roleManager);
}

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication(); // Make sure to include this for Identity
app.UseAuthorization();

// Configure routing for controllers
app.MapControllerRoute(
    name: "admin",
    pattern: "admin/{controller=Admin}/{action=Index}/{id?}");

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();

// Seed roles if they don't exist
static async Task SeedRolesAsync(IServiceProvider serviceProvider, RoleManager<IdentityRole> roleManager)
{
    string[] roleNames = { "Admin", "Company", "Jobseeker" };

    foreach (var roleName in roleNames)
    {
        var roleExist = await roleManager.RoleExistsAsync(roleName);
        if (!roleExist)
        {
            await roleManager.CreateAsync(new IdentityRole(roleName));
        }
    }
}





