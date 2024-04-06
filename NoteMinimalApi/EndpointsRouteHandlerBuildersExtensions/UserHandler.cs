using Microsoft.AspNetCore.Mvc;
using NoteMinimalApi.EndpointFilters;
using NoteMinimalApi.Services;
using NoteMinimalApi.ValidationProblems;
using NoteMinimalApi.ViewModels.UserViewModel;

namespace NoteMinimalApi.EndpointsRouteHandlerBuildersExtensions;

public static class UserHandler
{
    public static void MapUserEndpoints(this IEndpointRouteBuilder endpoint) 
    {
        var userApi = endpoint.MapGroup("/api/v1/user")
            .WithParameterValidation()
            .WithOpenApi()
            .WithTags("User");
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


    }
}