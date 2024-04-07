
namespace NoteMinimalApi.EndpointFilters;

public class LoggerFilter : IEndpointFilter
{
    public async ValueTask<object?> InvokeAsync(EndpointFilterInvocationContext context, EndpointFilterDelegate next)
    {
        var logger = context.HttpContext.RequestServices.GetService<ILogger<LoggerFilter>>()!;
        logger.LogInformation("Executing filer....");
        var result = await next(context);
        logger.LogInformation($"Handle result : {result}");
        return result;
 
    }
}