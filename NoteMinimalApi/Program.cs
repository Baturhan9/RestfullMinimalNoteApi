using Microsoft.EntityFrameworkCore;
using NoteMinimalApi.EndpointsRouteHandlerBuildersExtensions;
using NoteMinimalApi.Models;
using NoteMinimalApi.ServicesExtensions;

var builder = WebApplication.CreateBuilder(args);

string conString = builder.Configuration.GetConnectionString("DefaultConnection")!;
builder.Services.AddDbContext<AppDbContext>(options => options.UseNpgsql(conString));

builder.Services.AddApiServices();
builder.Services.AddProblemDetails();

builder.Configuration.Sources.Clear();
builder.Configuration.AddJsonFile("appsettings.json", optional: true);

if(builder.Environment.IsDevelopment())
{
    builder.Configuration.AddUserSecrets<Program>();
    builder.Services.AddSwaggerDocumentation(); 
}


var app = builder.Build();

if(app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseExceptionHandler();
app.UseStatusCodePages();


app.MapNoteEndpoints();

app.MapUserEndpoints();




app.Run();
