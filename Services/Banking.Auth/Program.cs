using Banking.Auth.Managers;
using Banking.Auth.Repositories;
using Serilog;
using Banking.Auth.Logger;
using Banking.Auth.HelperClasses;

var builder = WebApplication.CreateBuilder(args);

// Load configuration
string routefile = string.Format("appsettings.{0}.json", builder.Environment.EnvironmentName);
builder.Configuration.SetBasePath(builder.Environment.ContentRootPath)
    .AddJsonFile(routefile, optional: false, reloadOnChange: true)
    .AddEnvironmentVariables();

// Configure Serilog
builder.Host.UseSerilog((context, configuration) =>
    configuration.ReadFrom.Configuration(context.Configuration));

// Add services to the container
builder.Services.AddControllers();
builder.Services.AddSwaggerGen();
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddHttpContextAccessor();
builder.Services.AddEndpointsApiExplorer();

// Dependency Injection
builder.Services.AddTransient<IAuthManager, AuthManager>();
builder.Services.AddTransient<IAuthLogger, AuthLogger>();
builder.Services.AddTransient<IAuthRepository, AuthRepository>();


var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();
app.UseExceptionHandler(_ => { });
app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();
