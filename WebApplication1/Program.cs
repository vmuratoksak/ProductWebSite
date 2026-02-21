using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Localization;
using System.Globalization;
using MongoDB.Driver;
using WebApplication1.Models;
using WebApplication1.Models.Entities;
using WebApplication1.Repositories;
using WebApplication1.Repositories.Interfaces;
using WebApplication1.Services;
using WebApplication1.Services.Interfaces;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();

// 🌍 LOCALIZATION
builder.Services.AddLocalization(options => options.ResourcesPath = "Resources");

builder.Services.AddControllersWithViews()
    .AddViewLocalization()
    .AddDataAnnotationsLocalization();

var supportedCultures = new[]
{
    new CultureInfo("tr-TR"),
    new CultureInfo("en-US")
};

builder.Services.Configure<RequestLocalizationOptions>(options =>
{
    options.DefaultRequestCulture = new RequestCulture("tr-TR");
    options.SupportedCultures = supportedCultures;
    options.SupportedUICultures = supportedCultures;
});

// AUTH
builder.Services.AddAuthentication("MyCookie")
    .AddCookie("MyCookie", options =>
    {
        options.LoginPath = "/Auth/Login";
        options.AccessDeniedPath = "/Auth/Login";
    });

builder.Services.AddAuthorization();

// SESSION
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

builder.Services.AddHttpContextAccessor();

// MONGO
builder.Services.Configure<MongoSettings>(
    builder.Configuration.GetSection("MongoSettings"));

builder.Services.AddSingleton<IMongoClient>(sp =>
{
    var settings = sp.GetRequiredService<IOptions<MongoSettings>>().Value;
    return new MongoClient(settings.ConnectionString);
});

// REPOSITORIES
builder.Services.AddScoped<IRepository<ProductEntity>>(sp =>
    new GenericRepository<ProductEntity>(
        sp.GetRequiredService<IMongoClient>(),
        sp.GetRequiredService<IOptions<MongoSettings>>(),
        "Products"));

builder.Services.AddScoped<IRepository<CartEntity>>(sp =>
    new GenericRepository<CartEntity>(
        sp.GetRequiredService<IMongoClient>(),
        sp.GetRequiredService<IOptions<MongoSettings>>(),
        "Carts"));

builder.Services.AddScoped<IRepository<OrderEntity>>(sp =>
    new GenericRepository<OrderEntity>(
        sp.GetRequiredService<IMongoClient>(),
        sp.GetRequiredService<IOptions<MongoSettings>>(),
        "Orders"));

builder.Services.AddScoped<IRepository<UserEntity>>(sp =>
    new GenericRepository<UserEntity>(
        sp.GetRequiredService<IMongoClient>(),
        sp.GetRequiredService<IOptions<MongoSettings>>(),
        "Users"));

builder.Services.AddScoped<IRepository<NameEntity>>(sp =>
    new GenericRepository<NameEntity>(
        sp.GetRequiredService<IMongoClient>(),
        sp.GetRequiredService<IOptions<MongoSettings>>(),
        "Names"));

// SERVICES
builder.Services.AddScoped<IOrderService, OrderService>();
builder.Services.AddScoped<ICartService, CartService>();
builder.Services.AddScoped<IProductService, ProductService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IHomeService, HomeService>();
builder.Services.AddScoped<INameService, NameService>();

var app = builder.Build();

app.UseHttpsRedirection();
app.UseStaticFiles();

// 🌍 LOCALIZATION MIDDLEWARE (Routing’den ÖNCE)
var locOptions = app.Services.GetRequiredService<IOptions<RequestLocalizationOptions>>();
app.UseRequestLocalization(locOptions.Value);

app.UseRouting();

app.UseAuthentication();
app.UseSession();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();