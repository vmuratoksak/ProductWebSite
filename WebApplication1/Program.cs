using Microsoft.Extensions.Options;
using MongoDB.Driver;
using WebApplication1.Models;
using WebApplication1.Models.Entities;
using WebApplication1.Repositories;
using WebApplication1.Repositories.Interfaces;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();

// SESSION EKLE
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});
builder.Services.AddHttpContextAccessor();

// Mongo ayarları
builder.Services.Configure<MongoSettings>(
    builder.Configuration.GetSection("MongoSettings"));

builder.Services.AddSingleton<IMongoClient>(sp =>
{
    var settings = sp.GetRequiredService<IOptions<MongoSettings>>().Value;
    return new MongoClient(settings.ConnectionString);
});

var app = builder.Build();

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();


// SESSION MIDDLEWARE ÇOK ÖNEMLİ
app.UseSession();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

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

builder.Services.AddScoped<IOrderService, OrderService>();

app.Run();
