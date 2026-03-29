using Lab_03.Infrastructure.Seed;
using Lab_03.Infrastructure;
using Lab_03.Repositories;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Lab_03.DataAccess;
using Lab_03.Models;

var builder = WebApplication.CreateBuilder(args);

// MVC
builder.Services.AddControllersWithViews();

// Database
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection")
    )
);

// Identity — chỉ dùng AddIdentity (hỗ trợ Role), bỏ AddDefaultIdentity bị trùng
builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
{
    options.SignIn.RequireConfirmedAccount = false; // đổi thành true nếu muốn xác nhận email
})
.AddDefaultTokenProviders()
.AddDefaultUI()
.AddEntityFrameworkStores<ApplicationDbContext>();

builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Identity/Account/Login";
    options.LogoutPath = "/Identity/Account/Logout";
    options.AccessDeniedPath = "/Identity/Account/AccessDenied";
});

builder.Services.AddRazorPages();

// Repository DI
builder.Services.AddScoped<ITourRepository, EFTourRepository>();
builder.Services.AddScoped<IDestinationRepository, EFDestinationRepository>();
builder.Services.AddScoped<IBookingRepository, EFBookingRepository>();

// Booking payment (installment/down payment/refund demo)
builder.Services.Configure<BookingPaymentOptions>(builder.Configuration.GetSection("Booking"));
builder.Services.AddScoped<BookingPaymentCalculator>();

var app = builder.Build();

// Middleware pipeline
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
}

app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapRazorPages();

// 1. Route cho Area (phải đặt trước route default)
app.MapControllerRoute(
    name: "areas",
    pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}"
);

// 2. Route default (trang khách hàng)
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}"
);

await IdentityDataSeeder.SeedAsync(app.Services, app.Configuration, app.Logger);
await DestinationDataSeeder.SeedAsync(app.Services, app.Logger);

app.Run();
