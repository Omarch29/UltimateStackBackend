using System.Net;
using System.Reflection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace REPM.Application;

public static class ApplicationExtensions
{
    // Inject Services
    public static IServiceCollection AddApplicationDependencies(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddMediatR(cfg => { cfg.RegisterServicesFromAssemblies(AppDomain.CurrentDomain.GetAssemblies()); });
        services.AddAutoMapper(Assembly.GetExecutingAssembly());

        return services;
    }
}