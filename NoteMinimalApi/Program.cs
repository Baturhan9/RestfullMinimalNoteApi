using Microsoft.EntityFrameworkCore;
using NoteMinimalApi.EndpointFilters;
using NoteMinimalApi.Models;
using NoteMinimalApi.Services;
using NoteMinimalApi.ViewModels.NoteViewModel;
using NoteMinimalApi.ViewModels.UserViewModel;

var builder = WebApplication.CreateBuilder(args);

string conString = builder.Configuration.GetConnectionString("DefaultConnection")!;
builder.Services.AddDbContext<AppDbContext>(options => options.UseNpgsql(conString));
builder.Services.AddScoped<NoteService>();
builder.Services.AddScoped<UserService>();
builder.Services.AddProblemDetails();

builder.Configuration.Sources.Clear();
builder.Configuration.AddJsonFile("appsettings.json", optional: true);

if(builder.Environment.IsDevelopment())
{
    builder.Configuration.AddUserSecrets<Program>();

    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen();
}
else
{
}



var app = builder.Build();

if(app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseExceptionHandler();
app.UseStatusCodePages();


var api = app.MapGroup("/api/v1").WithParameterValidation();
var noteApi = api.MapGroup("/note").WithTags("Note");
var userApi = api.MapGroup("/user").WithTags("User");

noteApi.MapGet("/", async (NoteService service)=>
{
    return await service.GetNotesAsync();
});

noteApi.MapGet("/{id:int}", async (int id, NoteService service)=>
{
    var note = await service.GetNoteDetailAsync(id);
    return note is null 
    ? Results.Problem(statusCode:404)
    : Results.Ok(note);
})
.AddEndpointFilter<IdValidationFilter>();

noteApi.MapPost("/", async (CreateNoteCommand input, NoteService service)=>
{
    if(await service.IsAvailableForCreate(input.UserId))
    {
        var id = await service.CreateNoteAsync(input);
        return Results.Created("note", id);
    }

    return Results.ValidationProblem(new Dictionary<string, string[]>
    {
        {"userId", new[] {$"user with {input.UserId} id not exist"}}
    });
});

noteApi.MapPut("/", async (UpdateNoteCommand input, NoteService service) =>
{
    if(await service.IsAvailableForUpdate(input.NoteId)) 
    {
        await service.UpdateNoteAsync(input);
        return Results.NoContent();
    }
    return Results.Problem(statusCode: 404);
});

noteApi.MapDelete("/{id:int}", async (int id, NoteService service)=>
{
    await service.DeleteNoteASync(id);
    return Results.NoContent();
})
.AddEndpointFilter<IdValidationFilter>();

//----------------------------------------------------------------

userApi.MapGet("/", async (UserService service) =>
{
    return await service.GetUsersAsync();
});

userApi.MapGet("/{id:int}", async (int id, UserService service)=>
{
    var user = await service.GetUserDetailAsync(id);
    return user is null 
    ? Results.Problem(statusCode: 404)
    : Results.Ok(user);
})
.AddEndpointFilter<IdValidationFilter>();

userApi.MapPost("/", async (CreateUserCommand input, UserService service)=>
{
    if(await service.IsAvailableForCreate(input.Login))
    {
        var id = await service.CreateUserAsync(input);
        return Results.Created($"user", id);
    }
    return Results.ValidationProblem(new Dictionary<string, string[]>
    {
        {"login", new[]{$"user with '{input.Login}' login already exist"}}
    });
});

userApi.MapPut("/", async (UpdateUserCommand input, UserService service)=>
{
    if(await service.IsAvailableForUpdate(input.UserId))
    {
        await service.UpdateUserAsync(input);
        return Results.NoContent();
    }
    return Results.Problem(statusCode:404);
});

userApi.MapDelete("/{id:int}", async (int id, UserService service)=>
{
    await service.DeleteUserAsync(id);
    return Results.NoContent();
})
.AddEndpointFilter<IdValidationFilter>();

app.Run();
