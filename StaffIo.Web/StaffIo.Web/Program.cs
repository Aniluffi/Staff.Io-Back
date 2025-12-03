using Client.Files.Extensions;
using Microsoft.AspNetCore.Diagnostics;
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
        // »спользование ReferenceHandler.IgnoreCycles предотвращает циклические ссылки
        // при сериализации JSON. Ёто часто устран€ет проблему 500 в Swagger.
        options.JsonSerializerOptions.ReferenceHandler =
            System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
    });
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    // ѕолучаем им€ XML-файла (например, StaffIo.Web.xml)
    var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFilename);

    // ƒобавл€ем XML-документацию, но с проверкой, чтобы избежать ошибки 500
    if (File.Exists(xmlPath))
    {
        c.IncludeXmlComments(xmlPath);
    }
    else
    {
        // ќпционально: вывод сообщени€, если файл не найден
        Console.WriteLine($"WARNING: XML Documentation file not found at {xmlPath}");
    }
});

builder.Services.Configure<JwtOptions>(configuration.GetSection(nameof(JwtOptions)));

builder.Services.AddTransient<IJwtInternalService, JwtInternalService>();
builder.Services.AddTransient<StaffIo.IService.IAuthorizationService, AuthorizationService>();
builder.Services.AddTransient<StaffIo.IService.IExpensesService, ExpensesService>();
builder.Services.AddTransient<StaffIo.IService.IAnalyticsService, AnalyticsService>();
builder.Services.AddTransient<IEmployeesService,EmployeesService>();
builder.Services.AddTransient<IHistoryService,HistoryService>();

builder.Services.AddTransient<IAdminService, AdminService>();

builder.Services.AddFileService(configuration);

builder.Services.AddCors(options =>
{
    options.AddPolicy("SpecificDomain", policy =>
    {
        policy.WithOrigins("http://localhost:3000") // ? твой домен
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials(); // если используешь куки или авторизацию
    });
});

builder.Services.AddHttpContextAccessor();

var connectionString = configuration.GetSection(nameof(DataContext)).Get<string>();

builder.Services.AddDbContext<DataContext>(options => options.UseSqlServer(connectionString));



var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

//app.UseHttpsRedirection();

app.UseAuthorization();

app.UseCors("SpecificDomain");

app.MapControllers();

app.UseExceptionHandler(errorApp =>
{
    errorApp.Run(async context =>
    {
        context.Response.StatusCode = StatusCodes.Status500InternalServerError;
        context.Response.ContentType = "application/json";

        var error = context.Features.Get<IExceptionHandlerFeature>()?.Error;

        var result = System.Text.Json.JsonSerializer.Serialize(new
        {
            message = error?.Message,
            type = error?.GetType().Name
        });

        await context.Response.WriteAsync(result);
    });
});

app.Run();
