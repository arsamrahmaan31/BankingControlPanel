using Banking.Auth.HelperClasses;
using Banking.Auth.Logger;
using Banking.Auth.Managers;
using Banking.Auth.Repositories;
using Serilog;
var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddHttpContextAccessor();

// Determine the configuration file based on the environment
string routefile = string.Format("appsettings.{0}.json", builder.Environment.EnvironmentName);

// Set configuration sources: JSON file and environment variables
builder.Configuration.SetBasePath(builder.Environment.ContentRootPath)
    .AddJsonFile(routefile, optional: false, reloadOnChange: true).AddEnvironmentVariables();

// Configure Serilog using settings from the configuration file
builder.Host.UseSerilog((context, configuration) =>
configuration.ReadFrom.Configuration(context.Configuration));

// Registering Managers and Repositories with dependency injection (DI) as a transient service
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddTransient<IAuthManager, AuthManager>();
builder.Services.AddTransient<IAuthRepository, AuthRepository>();
builder.Services.AddTransient<IAuthLogger, AuthLogger>();


var app = builder.Build();
app.UseExceptionHandler(_ => { });
// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.Run();

