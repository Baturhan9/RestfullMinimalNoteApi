using Microsoft.EntityFrameworkCore;
using NoteMinimalApi.Models;
using NoteMinimalApi.Services;
using NoteMinimalApi.ViewModels.NoteViewModel;
using NoteMinimalApi.ViewModels.UserViewModel;

var builder = WebApplication.CreateBuilder(args);

string conString = builder.Configuration.GetConnectionString("DefaultConnection")!;
builder.Services.AddDbContext<AppDbContext>(options => options.UseNpgsql(conString));
builder.Services.AddScoped<NoteService>();
builder.Services.AddScoped<UserService>();

builder.Configuration.Sources.Clear();
builder.Configuration.AddJsonFile("appsettings.json", optional: true);

if(builder.Environment.IsDevelopment())
{
    builder.Configuration.AddUserSecrets<Program>();

    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen();
}


var app = builder.Build();

if(app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}


var api = app.MapGroup("/api/v1");
var noteApi = api.MapGroup("/note").WithTags("Note");
var userApi = api.MapGroup("/user").WithTags("User");

noteApi.MapGet("/", async (NoteService service)=>
{
    return await service.GetNotesAsync();
});

noteApi.MapGet("/{id:int}", async (int id, NoteService service)=>
{
    return await service.GetNoteDetailAsync(id);
});

noteApi.MapPost("/", async (CreateNoteCommand input, NoteService service)=>
{
    await service.CreateNoteAsync(input);
    return Results.Created();
});

noteApi.MapPut("/", async (UpdateNoteCommand input, NoteService service) =>
{
    await service.UpdateNoteAsync(input, DateTime);
    return Results.NoContent();
});

noteApi.MapDelete("/{id:int}", async (int id, NoteService service)=>
{
    await service.DeleteNoteASync(id);
    return Results.NoContent();
});


userApi.MapGet("/", async (UserService service) =>
{
    return await service.GetUsersAsync();
});

userApi.MapGet("/{id:int}", async (int id, UserService service)=>
{
    return await service.GetUserDetailAsync(id);
});

userApi.MapPost("/", async (CreateUserCommand input, UserService service)=>
{
    await service.CreateUserAsync(input);
    return Results.Created();
});

userApi.MapPut("/", async (UpdateUserCommand input, UserService service)=>
{
    await service.UpdateUserAsync(input);
    return Results.NoContent();
});

userApi.MapDelete("/", async (int id, UserService service)=>
{
    await service.DeleteUserAsync(id);
    return Results.NoContent();
});

app.Run();
