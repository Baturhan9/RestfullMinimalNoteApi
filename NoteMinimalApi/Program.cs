using Microsoft.EntityFrameworkCore;
using NoteMinimalApi.Models;

var builder = WebApplication.CreateBuilder(args);

string conString = builder.Configuration.GetConnectionString("DefaultConnection")!;
builder.Services.AddDbContext<AppDbContext>(options => options.UseNpgsql(conString));

builder.Configuration.Sources.Clear();
builder.Configuration.AddJsonFile("appsettings.json", optional: true);

if(builder.Environment.IsDevelopment())
{
    builder.Configuration.AddUserSecrets<Program>();
}


var app = builder.Build();

app.MapGet("/", (IConfiguration config) => config.AsEnumerable());

app.Run();
