using Banking.Client.ClientLogger;
using Banking.Client.HelperHandlers;
using Banking.Client.Managers;
using Banking.Client.Repositories;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Serilog;
using Swashbuckle.AspNetCore.Filters;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddHttpContextAccessor();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddAuthorization();
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddTransient<IClientLogger, ClientLogger>();
builder.Services.AddTransient<IClientManager, ClientManager>();
builder.Services.AddTransient<IClientRepository, ClientRepository>();
builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition("oauth2", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey
    });
    options.OperationFilter<SecurityRequirementsOperationFilter>();
});
builder.Services.AddAuthentication().AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        ValidateAudience = false,
        ValidateIssuer = false,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(
            builder.Configuration.GetSection("AppSettings:Token").Value!))
    };
});

// Determine the configuration file based on the environment
string routefile = string.Format("appsettings.{0}.json", builder.Environment.EnvironmentName);

// Set configuration sources: JSON file and environment variables
builder.Configuration.SetBasePath(builder.Environment.ContentRootPath)
    .AddJsonFile(routefile, optional: false, reloadOnChange: true).AddEnvironmentVariables();

// Configure Serilog using settings from the configuration file
builder.Host.UseSerilog((context, configuration) =>
configuration.ReadFrom.Configuration(context.Configuration));


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

