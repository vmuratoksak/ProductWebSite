using Microsoft.Extensions.Options;
using MongoDB.Driver;
using SatisSitesi.Models;
using SatisSitesi.Domain.Entities;
using SatisSitesi.Infrastructure.Repositories;
using SatisSitesi.Application.Interfaces.Repositories;
using SatisSitesi.Application.Services;
using SatisSitesi.Application.Interfaces.Services;
using Microsoft.AspNetCore.Mvc.Razor;

var builder = WebApplication.CreateBuilder(args);

// LOCALIZATION
builder.Services.AddLocalization();

builder.Services.Configure<RequestLocalizationOptions>(options =>
{
    var supportedCultures = new[] { "tr", "en", "de", "fr", "ar" };
    options.SetDefaultCulture(supportedCultures[0])
        .AddSupportedCultures(supportedCultures)
        .AddSupportedUICultures(supportedCultures);
});

builder.Services.AddControllersWithViews()
    .AddViewLocalization(LanguageViewLocationExpanderFormat.Suffix)
    .AddDataAnnotationsLocalization();

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


// MONGO CONFIG
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

builder.Services.AddScoped<IRepository<TranslationCacheEntity>>(sp =>
    new GenericRepository<TranslationCacheEntity>(
        sp.GetRequiredService<IMongoClient>(),
        sp.GetRequiredService<IOptions<MongoSettings>>(),
        "TranslationCache"));

// SERVICES
builder.Services.AddScoped<IOrderService, OrderService>();
builder.Services.AddScoped<ICartService, CartService>();
builder.Services.AddScoped<IProductService, ProductService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IHomeService, HomeService>();
builder.Services.AddScoped<ICustomerService, CustomerService>();
builder.Services.AddScoped<ICurrencyService, CurrencyService>();

// TRANSLATION API (DI)
builder.Services.AddHttpClient<ITranslationService, MyMemoryTranslationService>();


var app = builder.Build();

app.UseHttpsRedirection();
app.UseStaticFiles();

// LOCALIZATION MIDDLEWARE — must be before UseRouting
app.UseRequestLocalization();

app.UseRouting();

// Auth pipeline
app.UseAuthentication();
app.UseSession();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
