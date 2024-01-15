using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using StoringPassword.Models;
using StoringPassword.Repos;
using System.Configuration;

var builder = WebApplication.CreateBuilder(args);

// Все сессии работают поверх объекта IDistributedCache, и 
// ASP.NET Core предоставляет встроенную реализацию IDistributedCache
builder.Services.AddDistributedMemoryCache();// добавляем IDistributedMemoryCache
builder.Services.AddSession();  // Добавляем сервисы сессии

// Получаем строку подключения из файла конфигурации
string? connection = builder.Configuration.GetConnectionString("DefaultConnection");

// добавляем контекст ApplicationContext в качестве сервиса в приложение
builder.Services.AddDbContext<UserContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
    options.LogTo(Console.WriteLine, LogLevel.Information); // Логирование SQL запросов
});

builder.Services.AddScoped<IMessagesRepository, MessagesRepository>();
builder.Services.AddScoped<IAccountsRepository, AccountsRepository>();

// Добавляем сервисы MVC
builder.Services.AddControllersWithViews();


var app = builder.Build();
app.UseSession();   // Добавляем middleware-компонент для работы с сессиями
app.UseStaticFiles(); // обрабатывает запросы к файлам в папке wwwroot

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
