using Jobportalwebsite.Data;
using Jobportalwebsite.Models;
using Jobportalwebsite.Services;
using Jobportalwebsite.IHelper;
using Jobportalwebsite.Helper;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
ConfigureServices(builder.Services, builder.Configuration);

var app = builder.Build();

// Apply migrations and seed roles at startup.
await ApplyMigrationsAndSeedRolesAsync(app);

// Configure middleware and request pipeline.
ConfigureMiddleware(app);

app.Run();

// Method to configure services
void ConfigureServices(IServiceCollection services, IConfiguration configuration)
{
    services.AddControllersWithViews();
    services.AddRazorPages().AddRazorRuntimeCompilation();
    services.AddScoped<IUserHelper, UserHelper>();
    services.AddScoped<NotificationService>();

    // Configure ApplicationDbContext
    services.AddDbContext<ApplicationDbContext>(options =>
        options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));

    // Configure Identity
    services.AddDefaultIdentity<ApplicationUser>(options =>
    {
        options.Password.RequireDigit = false;
        options.Password.RequireLowercase = false;
        options.Password.RequireUppercase = false;
        options.Password.RequireNonAlphanumeric = false;
        options.Password.RequiredLength = 1;
        options.Password.RequiredUniqueChars = 1;
    })
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders();
}

// Method to apply migrations and seed roles
async Task ApplyMigrationsAndSeedRolesAsync(WebApplication app)
{
    using var scope = app.Services.CreateScope();
    var services = scope.ServiceProvider;

    var context = services.GetRequiredService<ApplicationDbContext>();
    var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();

    await context.Database.MigrateAsync(); // Apply migrations
    await SeedRolesAsync(roleManager);     // Seed roles
}

// Method to configure middleware and routing
void ConfigureMiddleware(WebApplication app)
{
    if (!app.Environment.IsDevelopment())
    {
        app.UseExceptionHandler("/Home/Error");
        app.UseHsts();
    }

    app.UseHttpsRedirection();
    app.UseStaticFiles();
    app.UseRouting();
    app.UseAuthentication();
    app.UseAuthorization();

    // Routing configuration
    app.MapControllerRoute(
        name: "admin",
        pattern: "admin/{controller=Admin}/{action=Index}/{id?}");

    app.MapControllerRoute(
        name: "default",
        pattern: "{controller=Home}/{action=Index}/{id?}");
}

// Method to seed roles if they don't exist
static async Task SeedRolesAsync(RoleManager<IdentityRole> roleManager)
{
    string[] roleNames = { "Admin", "Company", "Jobseeker" };

    foreach (var roleName in roleNames)
    {
        if (!await roleManager.RoleExistsAsync(roleName))
        {
            await roleManager.CreateAsync(new IdentityRole(roleName));
        }
    }
}




//using Jobportalwebsite.Data; // Access ApplicationDbContext and NotificationService
//using Jobportalwebsite.Models; // Access ApplicationUser
//using Microsoft.EntityFrameworkCore; // Required for EF Core
//using Microsoft.AspNetCore.Identity; // If using Identity
//using Microsoft.Extensions.DependencyInjection; // Required for DI
//using System;
//using System.Threading.Tasks;
//using Jobportalwebsite.Services;
//using Jobportalwebsite.IHelper;
//using Jobportalwebsite.Helper; 


//var builder = WebApplication.CreateBuilder(args);

//// Add services to the container.
//builder.Services.AddControllersWithViews();
//builder.Services.AddScoped<IUserHelper, UserHelper>();
//// Add Razor Pages with runtime compilation
//builder.Services.AddRazorPages().AddRazorRuntimeCompilation();

//// Configure ApplicationDbContext with SQL Server
//builder.Services.AddDbContext<ApplicationDbContext>(options =>
//    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

//// Add the NotificationService
//builder.Services.AddScoped<NotificationService>();

//// Add Identity services with ApplicationUser
//builder.Services.AddDefaultIdentity<ApplicationUser>(options =>
//{
//    // Disable specific password requirements
//    options.Password.RequireDigit = false; // No digits required
//    options.Password.RequireLowercase = false; // No lowercase letters required
//    options.Password.RequireUppercase = false; // No uppercase letters required
//    options.Password.RequireNonAlphanumeric = false; // No special characters required
//    options.Password.RequiredLength = 1; // Minimum length is 1
//    options.Password.RequiredUniqueChars = 1; // Only 1 unique character required
//})
//.AddRoles<IdentityRole>() // Add roles to Identity
//.AddEntityFrameworkStores<ApplicationDbContext>()
//.AddDefaultTokenProviders();

//// Seed roles asynchronously at app startup
//var app = builder.Build();

//// Seed roles at application startup
//using (var scope = app.Services.CreateScope())
//{
//    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
//    await SeedRolesAsync(scope.ServiceProvider, roleManager);
//}

//// Configure the HTTP request pipeline.
//if (!app.Environment.IsDevelopment())
//{
//    app.UseExceptionHandler("/Home/Error");
//    app.UseHsts();
//}

//app.UseHttpsRedirection();
//app.UseStaticFiles();

//app.UseRouting();

//app.UseAuthentication(); // Make sure to include this for Identity
//app.UseAuthorization();

//// Configure routing for controllers
//app.MapControllerRoute(
//    name: "admin",
//    pattern: "admin/{controller=Admin}/{action=Index}/{id?}");

//app.MapControllerRoute(
//    name: "default",
//    pattern: "{controller=Home}/{action=Index}/{id?}");

//app.Run();

//// Seed roles if they don't exist
//static async Task SeedRolesAsync(IServiceProvider serviceProvider, RoleManager<IdentityRole> roleManager)
//{
//    string[] roleNames = { "Admin", "Company", "Jobseeker" };

//    foreach (var roleName in roleNames)
//    {
//        var roleExist = await roleManager.RoleExistsAsync(roleName);
//        if (!roleExist)
//        {
//            await roleManager.CreateAsync(new IdentityRole(roleName));
//        }
//    }
//}


