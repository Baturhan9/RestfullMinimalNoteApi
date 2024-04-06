using Microsoft.OpenApi.Models;

namespace NoteMinimalApi.ServicesExtensions;

public static class SwaggerServiceCollectionExtension 
{
    public static IServiceCollection AddSwaggerDocumentation(this IServiceCollection services) 
    {
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen(x=>
        {
            x.SwaggerDoc("v1", new OpenApiInfo()
            {
                Title = "Api for some app",
                Description = "api for interacting with users and notes on db server",
                Version = "alpha"
            });
        });
        

        return services;
    }
}

