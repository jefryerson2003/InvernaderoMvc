using ArcgisLoginApp.Data;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Cadena de conexión
var connectionString = "Server=tcp:arcgis-server.database.windows.net,1433;Initial Catalog=Arcgis-DataBase;Persist Security Info=False;User ID=CloudSA0f330583@arcgis-server;Password=AyanokojiKami2003;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;";

// EF Core
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));

// Autenticación por cookies
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Account/Login";
        options.AccessDeniedPath = "/Account/AccessDenied";
    });
// Configuramos un cliente para llamar a Node-RED en localhost:1880
builder.Services.AddHttpClient("NodeRed", client =>
{
    client.BaseAddress = new Uri("http://localhost:1880");
    client.Timeout = TimeSpan.FromSeconds(5);
});

builder.Services.AddHttpContextAccessor();

builder.Services.AddControllersWithViews();

var app = builder.Build();

app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Account}/{action=Login}/{id?}");

app.Run();
