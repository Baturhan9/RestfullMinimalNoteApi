
namespace NoteMinimalApi.EndpointFilters;

public class IdValidationFilter : IEndpointFilter
{
    public async ValueTask<object?> InvokeAsync(EndpointFilterInvocationContext context, EndpointFilterDelegate next)
    {
        int id = context.GetArgument<int>(0);
        if(id < 0)
        {
            return Results.ValidationProblem(
                new Dictionary<string, string[]>
                {
                    {"id", new[]{"id must be positive number"}}
                }
            );
        }
        return await next(context);
    }
}