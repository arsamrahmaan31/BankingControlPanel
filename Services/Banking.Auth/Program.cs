using Serilog;
var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
string routefile = string.Format("appsettings.{0}.json", builder.Environment.EnvironmentName);
builder.Configuration.SetBasePath(builder.Environment.ContentRootPath)
    .AddJsonFile(routefile, optional: false, reloadOnChange: true).AddEnvironmentVariables();


builder.Host.UseSerilog((context, configuration) =>
configuration.ReadFrom.Configuration(context.Configuration));


// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

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

