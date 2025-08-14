using Marten;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
// This removes the Server header that says "Kestrel"
builder.WebHost.ConfigureKestrel(serverOptions => { serverOptions.AddServerHeader = false; });

// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var connectionString = builder.Configuration.GetConnectionString("db") ??
                       throw new Exception("Need a connection string");
builder.Services.AddMarten(config =>
{
    config.Connection(connectionString);

}).UseLightweightSessions();

builder.Services.AddSingleton<TimeProvider>(_ => TimeProvider.System);
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}


app.UseDefaultFiles();
app.UseStaticFiles();
app.MapControllers();
app.Run();

public partial class Program;