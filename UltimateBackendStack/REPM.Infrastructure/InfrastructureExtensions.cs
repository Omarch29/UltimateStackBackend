using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using REPM.Infrastructure.Interfaces;
using REPM.Infrastructure.Persistence;
using REPM.Infrastructure.Repositories;

namespace REPM.Infrastructure;

public static class InfrastructureExtensions
{
    // Inject Services
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        var sqlConnectionString = Environment.GetEnvironmentVariable("REPM_CONNECTION_STRING") ??
                                  configuration.GetConnectionString("WebApiDatabase");
        services.AddDbContext<REPMContext>(options =>
        {
            options.UseNpgsql(sqlConnectionString);
            options.EnableSensitiveDataLogging();
        });

        services.AddHttpContextAccessor();
        
        services.AddScoped<IUnitOfWork, REPMContext>();
        services.AddTransient(typeof(IRepository<>), typeof(Repository<>));

        return services;
    }
    
    // MCP-specific infrastructure setup without logging
    public static IServiceCollection AddInfrastructureForMcp(this IServiceCollection services, IConfiguration configuration)
    {
        var sqlConnectionString = Environment.GetEnvironmentVariable("REPM_CONNECTION_STRING") ??
                                  configuration.GetConnectionString("WebApiDatabase");
        services.AddDbContext<REPMContext>(options =>
        {
            options.UseNpgsql(sqlConnectionString);
            // Completely disable all logging for MCP - STDIO must be pure JSON-RPC
            options.UseLoggerFactory(LoggerFactory.Create(builder => { }));
            options.EnableSensitiveDataLogging(false);
            options.EnableDetailedErrors(false);
            options.EnableServiceProviderCaching(false);
            options.ConfigureWarnings(warnings => warnings.Ignore());
        });

        services.AddHttpContextAccessor();
        
        services.AddScoped<IUnitOfWork, REPMContext>();
        services.AddTransient(typeof(IRepository<>), typeof(Repository<>));

        return services;
    }
}