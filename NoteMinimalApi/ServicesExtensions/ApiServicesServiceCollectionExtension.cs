using NoteMinimalApi.Services;

namespace NoteMinimalApi.ServicesExtensions;

public static class ApiServicesServiceCollectionExtension
{
    public static IServiceCollection AddApiServices(this IServiceCollection services) 
    {
        services.AddScoped<NoteService>();
        services.AddScoped<UserService>();
        
        return services;
    }
}