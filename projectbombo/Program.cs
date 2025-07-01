using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using projectbombo.Data;

using Microsoft.AspNetCore.Authentication.Cookies;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();

// MSSQL bağlantısı
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("mssqlconnection"))); // UseSqlite yerine UseSqlServer

// Kimlik doğrulama ekleyelim (çerez bazlı)
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/User/Login"; // Giriş sayfasına yönlendir
        options.AccessDeniedPath = "/User/AccessDenied"; // Yetkisiz girişlerde yönlendirme
    });

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var dbContext = services.GetRequiredService<AppDbContext>();
        dbContext.Database.Migrate();
        // İsterseniz burada seed data da ekleyebilirsiniz
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "Veritabanı migrations sırasında bir hata oluştu.");
    }
}

// Kimlik doğrulama ve yetkilendirme middleware'lerini doğru sırayla ekleyin
app.UseAuthentication(); // Authentication işlemleri
app.UseAuthorization();  // Authorization işlemleri

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting(); // Routing işlemleri

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
