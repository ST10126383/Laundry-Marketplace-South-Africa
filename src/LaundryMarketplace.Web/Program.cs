using LaundryMarketplace.Infrastructure.Data;
using LaundryMarketplace.Infrastructure.Identity;
using LaundryMarketplace.Infrastructure.Services;
using LaundryMarketplace.Core.Interfaces;
using LaundryMarketplace.Web.Middleware;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// Database - Cloud ready (Azure SQL / LocalDB / PostgreSQL connection string)
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? "Host=ep-weathered-star-b5035686.c-7.us-east-2.aws.neon.tech;Database=LaundryMarketplaceDb;Username=neondb_owner;Password=npg_DzgN2yCcn7Wv;SSL Mode=Require;Trust Server Certificate=true;Timeout=30";

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(connectionString));

// Identity
builder.Services.AddIdentity<ApplicationUserIdentity, IdentityRole>(options =>
{
    options.Password.RequireDigit = true;
    options.Password.RequireLowercase = true;
    options.Password.RequireUppercase = true;
    options.Password.RequireNonAlphanumeric = true;
    options.Password.RequiredLength = 8;
    options.User.RequireUniqueEmail = true;
    options.SignIn.RequireConfirmedAccount = false; // simplify for MVP
})
.AddEntityFrameworkStores<ApplicationDbContext>()
.AddDefaultTokenProviders();

builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Account/Login";
    options.AccessDeniedPath = "/Account/AccessDenied";
    options.ExpireTimeSpan = TimeSpan.FromHours(8);
});

// Application services
builder.Services.AddScoped<IOrderService, OrderService>();
builder.Services.AddScoped<INpoService, NpoService>();

// Authorization policies
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("RequireCustomer", p => p.RequireRole("Customer"));
    options.AddPolicy("RequireLaundromat", p => p.RequireRole("Laundromat"));
    options.AddPolicy("RequireNpoAdmin", p => p.RequireRole("NpoAdmin", "SystemAdmin"));
    options.AddPolicy("RequireSystemAdmin", p => p.RequireRole("SystemAdmin"));
});

var app = builder.Build();

// Global exception handling (must be early in the pipeline)
app.UseGlobalExceptionHandler();

// Configure the HTTP request pipeline.
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

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

// Seed database
using (var scope = app.Services.CreateScope())
{
    try
    {
        await DbSeeder.SeedAsync(scope.ServiceProvider);
    }
    catch (Exception ex)
    {
        var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "An error occurred while seeding the database.");
    }
}

app.Run();
