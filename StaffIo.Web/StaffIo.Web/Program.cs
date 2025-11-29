using Microsoft.EntityFrameworkCore;
using StaffIo.Data;
using StaffIo.IService;
using StaffIo.Service;
using StaffIo.Service.Options;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
IConfiguration configuration = new ConfigurationBuilder()
    .SetBasePath(Environment.CurrentDirectory)
    .AddJsonFile("appsettings.json", true, true)
    .Build();

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        // Использование ReferenceHandler.IgnoreCycles предотвращает циклические ссылки
        // при сериализации JSON. Это часто устраняет проблему 500 в Swagger.
        options.JsonSerializerOptions.ReferenceHandler =
            System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
    });
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    // Получаем имя XML-файла (например, StaffIo.Web.xml)
    var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFilename);

    // Добавляем XML-документацию, но с проверкой, чтобы избежать ошибки 500
    if (File.Exists(xmlPath))
    {
        c.IncludeXmlComments(xmlPath);
    }
    else
    {
        // Опционально: вывод сообщения, если файл не найден
        Console.WriteLine($"WARNING: XML Documentation file not found at {xmlPath}");
    }
});

builder.Services.Configure<JwtOptions>(configuration.GetSection(nameof(JwtOptions)));

builder.Services.AddTransient<IJwtInternalService, JwtInternalService>();
builder.Services.AddTransient<StaffIo.IService.IAuthorizationService, AuthorizationService>();
builder.Services.AddTransient<StaffIo.IService.IExpensesService, ExpensesService>();
builder.Services.AddTransient<StaffIo.IService.IAnalyticsService, AnalyticsService>();

builder.Services.AddHttpContextAccessor();

var connectionString = configuration.GetSection(nameof(DataContext)).Get<string>();

builder.Services.AddDbContext<DataContext>(options => options.UseSqlServer(connectionString));



var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
