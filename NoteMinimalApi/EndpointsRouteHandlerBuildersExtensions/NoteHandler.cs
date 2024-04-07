
using Microsoft.AspNetCore.Mvc;
using NoteMinimalApi.EndpointFilters;
using NoteMinimalApi.Services;
using NoteMinimalApi.ValidationProblems;
using NoteMinimalApi.ViewModels.NoteViewModel;

namespace NoteMinimalApi.EndpointsRouteHandlerBuildersExtensions;

public static class NoteHandler 
{
    public static void MapNoteEndpoints(this IEndpointRouteBuilder endpoint)
    {
        var noteApi = endpoint.MapGroup("/api/v1/note")
            .WithParameterValidation()
            .WithOpenApi()
            .WithTags("Note")
            .AddEndpointFilter<LoggerFilter>();

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


    }
}
