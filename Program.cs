using Microsoft.EntityFrameworkCore;
using SneakerShopMySQL.Data;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorPages();
builder.Services.AddDbContext<SneakerShopContext>(options =>
    options.UseMySql(builder.Configuration.GetConnectionString("SneakerShopContext"), ServerVersion.AutoDetect(builder.Configuration.GetConnectionString("SneakerShopContext"))));

builder.Services.AddDistributedMemoryCache();

builder.Services.AddSession(options =>
{
	options.IdleTimeout = TimeSpan.FromMinutes(20);
	options.Cookie.HttpOnly = true;
	options.Cookie.IsEssential = true;
});

builder.Logging.AddFile("Logs/SneakerShopLogs-{Date}.txt");

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
	app.UseExceptionHandler("/Error");
	app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.UseSession();

app.MapRazorPages();

app.Run();
