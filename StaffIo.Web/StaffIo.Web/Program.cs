using Microsoft.EntityFrameworkCore;
using StaffIo.Data;
using StaffIo.IService;
using StaffIo.Service;
using StaffIo.Service.Options;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
IConfiguration configuration = new ConfigurationBuilder()
    .SetBasePath(Environment.CurrentDirectory)
    .AddJsonFile("appsettings.json",true,true)
    .Build();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.Configure<JwtOptions>(configuration.GetSection(nameof(JwtOptions)));

builder.Services.AddTransient<IJwtInternalService, JwtInternalService>();
builder.Services.AddTransient<StaffIo.IService.IAuthorizationService, AuthorizationService>();

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
