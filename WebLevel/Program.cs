using Bussines.MailServices;
using Interfaces;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;
using Swashbuckle.AspNetCore.Filters;
using WebApplication1;

var builder = WebApplication.CreateBuilder(args);

// Настройка логирования
Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .WriteTo.File("logs/log.txt", rollingInterval: RollingInterval.Day)
    .CreateLogger();

builder.Host.UseSerilog();

string dbProxy = String.Empty ;

if (builder.Environment.IsDevelopment())
{
    builder.WebHost.ConfigureKestrel(options =>
    {
        options.ListenAnyIP(5072);
    });

    var configuration = builder.Configuration;
    dbProxy = configuration["ProxyMicroservice:BaseUrl"] ?? throw new Exception("DbProxy is missing");

    // Читаем SMTP настройки из appsettings.json
    builder.Services.Configure<SmtpSettings>(configuration.GetSection("SmtpSettings"));
}
else
{
    builder.WebHost.ConfigureKestrel(options =>
    {
        options.ListenAnyIP(8080);
    });

    dbProxy = Environment.GetEnvironmentVariable("BaseUrl") ?? throw new Exception("DbProxy is missing");

    // Читаем SMTP настройки из переменных окружения
    builder.Services.Configure<SmtpSettings>(options =>
    {
        options.SmtpServer = Environment.GetEnvironmentVariable("SmtpServer") ?? throw new Exception("SmtpServer is missing");
        Log.Information($"SmtpServer { options.SmtpServer}");
        if (!int.TryParse(Environment.GetEnvironmentVariable("Port"), out int port))
        {
            throw new Exception("Port is missing or invalid");
        }
        options.Port = port;
        Log.Information($"Port { options.Port}");
        options.Password = Environment.GetEnvironmentVariable("Password") ?? throw new Exception("Password is missing");
        Log.Information($"Password { options.Password}");
        options.FromEmail = Environment.GetEnvironmentVariable("FromEmail") ?? throw new Exception("FromEmail is missing");
        Log.Information($"FromEmail { options.FromEmail}");
    });
}

builder.Services.AddHttpClient("ProxyApiClient", client =>
{
    if (string.IsNullOrEmpty(dbProxy)) throw new Exception("dbProxy не инициализирован");
    client.BaseAddress = new Uri(dbProxy); 
});

// Добавляем сервисы
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c => c.EnableAnnotations());

builder.Services.AddHttpClient<ILogicSenderCong, LogicSender>();
builder.Services.AddTransient<IMailService, MailService>();
builder.Services.AddHostedService<MailBackgroundService>();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.UseAuthorization();
app.MapControllers();

Log.Information("Starting web application");
Log.Information($"dbProxy is listening on {dbProxy}");

app.Run();
