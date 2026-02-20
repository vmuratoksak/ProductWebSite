using Microsoft.Extensions.Options;
using MongoDB.Driver;
using WebApplication1.Models;
using WebApplication1.Models.Entities;
using WebApplication1.Repositories;
using WebApplication1.Repositories.Interfaces;
using WebApplication1.Services;
using WebApplication1.Services.Interfaces;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();

builder.Services.AddDistributedMemoryCache();

builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

builder.Services.AddHttpContextAccessor();


// 🔥 MONGO CONFIG
builder.Services.Configure<MongoSettings>(
    builder.Configuration.GetSection("MongoSettings"));

builder.Services.AddSingleton<IMongoClient>(sp =>
{
    var settings = sp.GetRequiredService<IOptions<MongoSettings>>().Value;
    return new MongoClient(settings.ConnectionString);
});


// 🔥 REPOSITORIES

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


// 🔥 EKSIK OLAN BUYDU — NAMEENTITY REPO
builder.Services.AddScoped<IRepository<NameEntity>>(sp =>
    new GenericRepository<NameEntity>(
        sp.GetRequiredService<IMongoClient>(),
        sp.GetRequiredService<IOptions<MongoSettings>>(),
        "Names"));


// 🔥 SERVICES
builder.Services.AddScoped<IOrderService, OrderService>();
builder.Services.AddScoped<ICartService, CartService>();
builder.Services.AddScoped<IProductService, ProductService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IHomeService, HomeService>();
builder.Services.AddScoped<INameService, NameService>();


var app = builder.Build();

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseSession();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();