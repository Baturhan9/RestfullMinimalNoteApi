using System.Reflection;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using NoteMinimalApi.EndpointFilters;
using NoteMinimalApi.Models;
using NoteMinimalApi.Services;
using NoteMinimalApi.ValidationProblems;
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
    builder.Services.AddSwaggerGen(x=>
    {
        x.SwaggerDoc("v1", new OpenApiInfo()
        {
            Title = "Api for some app",
            Description = "api for interacting with users and notes on db server",
            Version = "alpha"
        });
    });
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


var api = app.MapGroup("/api/v1").WithParameterValidation().WithOpenApi();
var noteApi = api.MapGroup("/note").WithTags("Note");
var userApi = api.MapGroup("/user").WithTags("User");

noteApi.MapGet("/", 
[EndpointName("Get list of notes")]
[EndpointSummary("Fetches a list of notes")]
[EndpointDescription("Fetches a list of notes")]
[ProducesResponseType(typeof(IEnumerable<NoteSummaryViewModel>), 200)]
async (NoteService service)=>
{
    return await service.GetNotesAsync();
});

noteApi.MapGet("/{id:int}", 
[EndpointName("Get note by id")]
[EndpointSummary("Fetches a note by id")]
[EndpointDescription("Fetches a note by id and returns 200 or returns 404 if note isn't found")]
[ProducesResponseType(typeof(NoteDetailViewModel), 200)]
[ProducesResponseType(typeof(HttpNotFoundProblem), 404)]
async (int id, NoteService service)=>
{
    var note = await service.GetNoteDetailAsync(id);
    return note is null 
    ? Results.Problem(statusCode:404)
    : Results.Ok(note);
})
.AddEndpointFilter<IdValidationFilter>();

noteApi.MapPost("/", 
[EndpointName("Create a note")]
[EndpointSummary("Create a note")]
[EndpointDescription("Create a note and returns 201 with id's note or returns 400 if input isn't valid or returns 404 if user isn't found")]
[ProducesResponseType(typeof(int), 200)]
[ProducesResponseType(typeof(HttpValidationProblemDetails), 400, "application/problem+json")]
[ProducesResponseType(typeof(HttpNotFoundProblem), 404)]
async (CreateNoteCommand input, NoteService service)=>
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

noteApi.MapPut("/", 
[EndpointName("Update note")]
[EndpointSummary("Update a note")]
[EndpointDescription("Update a note and returns 204 or returns 400 if input isn't valid or returns 404 if note isn't found")]
[ProducesResponseType(204)]
[ProducesResponseType(typeof(HttpValidationProblemDetails), 400, "application/problem+json")]
[ProducesResponseType(typeof(HttpNotFoundProblem), 404)]
async (UpdateNoteCommand input, NoteService service) =>
{
    if(await service.IsAvailableForUpdate(input.NoteId)) 
    {
        await service.UpdateNoteAsync(input);
        return Results.NoContent();
    }
    return Results.Problem(statusCode: 404);
});

noteApi.MapDelete("/{id:int}", 
[EndpointName("Delete a note")]
[EndpointSummary("Delete a note")]
[EndpointDescription("Change a note's filed isDelete and returns 204 or returns 404 if note isn't found")]
[ProducesResponseType(204)]
[ProducesResponseType(typeof(HttpNotFoundProblem), 404)]
async (int id, NoteService service)=>
{
    await service.DeleteNoteASync(id);
    return Results.NoContent();
})
.AddEndpointFilter<IdValidationFilter>();

//----------------------------------------------------------------

userApi.MapGet("/",
[EndpointName("Get a list of users")]
[EndpointSummary("Fetches a list of users")]
[EndpointDescription("Fetches a list of users and returns 200")]
[ProducesResponseType(typeof(IEnumerable<UserSummaryViewModel>), 200)]
async (UserService service) =>
{
    return await service.GetUsersAsync();
});

userApi.MapGet("/{id:int}", 
[EndpointName("Get a user by id")]
[EndpointSummary("Fetches a user by id")]
[EndpointDescription("Fetches a user by id and returns 200 or returns 404 if user isn't found")]
[ProducesResponseType(typeof(UserDetailViewModel), 200)]
[ProducesResponseType(typeof(HttpNotFoundProblem), 404)]
async (int id, UserService service)=>
{
    var user = await service.GetUserDetailAsync(id);
    return user is null 
    ? Results.Problem(statusCode: 404)
    : Results.Ok(user);
})
.AddEndpointFilter<IdValidationFilter>();

userApi.MapPost("/", 
[EndpointName("Create a user")]
[EndpointSummary("Create a user")]
[EndpointDescription("Create a note and returns 201 with user's id or returns 400 if input isn't valid")]
[ProducesResponseType(typeof(int),201)]
[ProducesResponseType(typeof(HttpValidationProblemDetails), 400, "application/problem+json")]
async (CreateUserCommand input, UserService service)=>
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

userApi.MapPut("/", 
[EndpointName("Update user")]
[EndpointSummary("Update a user")]
[EndpointDescription("Update a user and returns 204 or returns 400 if input isn't valid or returns 404 if user isn't found")]
[ProducesResponseType(204)]
[ProducesResponseType(typeof(HttpValidationProblemDetails), 400, "application/problem+json")]
[ProducesResponseType(typeof(HttpNotFoundProblem), 404)]
async (UpdateUserCommand input, UserService service)=>
{
    if(await service.IsAvailableForUpdate(input.UserId))
    {
        await service.UpdateUserAsync(input);
        return Results.NoContent();
    }
    return Results.Problem(statusCode:404);
});

userApi.MapDelete("/{id:int}",
[EndpointName("Delete a user")]
[EndpointSummary("Delete a user")]
[EndpointDescription("Changes the user's filed isDelete in true and returns 204 or returns 404 if user isn't found")]
[ProducesResponseType(204)]
[ProducesResponseType(typeof(HttpNotFoundProblem), 404)]
async (int id, UserService service)=>
{
    await service.DeleteUserAsync(id);
    return Results.NoContent();
})
.AddEndpointFilter<IdValidationFilter>();

app.Run();
