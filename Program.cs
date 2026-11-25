using HotelBookingSystem.Data;
using HotelBookingSystem.Interfaces;
using HotelBookingSystem.Models;
using HotelBookingSystem.Services;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// Configure Entity Framework Core with Oracle's official MySQL provider.
// Store your real password in User Secrets or environment variables (ConnectionStrings__DefaultConnection).
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseMySQL(connectionString));

// Configure secure cookie authentication for guests and administrators.
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Account/Login";
        options.AccessDeniedPath = "/Account/AccessDenied";
        options.Cookie.Name = "HotelBooking.Auth";
        options.Cookie.HttpOnly = true;
        options.Cookie.SameSite = SameSiteMode.Lax;
        options.Cookie.SecurePolicy = CookieSecurePolicy.SameAsRequest;
        options.ExpireTimeSpan = TimeSpan.FromHours(8);
        options.SlidingExpiration = true;
    });

// Register application services, password hasher, and database initializer.
builder.Services.AddScoped<PasswordHasher<ApplicationUser>>();
builder.Services.AddScoped<IPasswordHasherService, PasswordHasherService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IAdminService, AdminService>();
builder.Services.AddScoped<IRoomService, RoomService>();
builder.Services.AddScoped<IBookingService, BookingService>();
builder.Services.AddScoped<DbInitializer>();

var app = builder.Build();

// Execute idempotent database seeding on startup (without running automatic migrations).
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var logger = services.GetRequiredService<ILogger<Program>>();
    try
    {
        var initializer = services.GetRequiredService<DbInitializer>();
        await initializer.InitializeAsync();
    }
    catch (Exception ex)
    {
        logger.LogError(ex, "An error occurred while initializing the database.");
    }
}

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "areas",
    pattern: "{area:exists}/{controller=Dashboard}/{action=Index}/{id?}");

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();

app.Run();
