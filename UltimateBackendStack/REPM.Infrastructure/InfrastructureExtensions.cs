using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using REPM.Infrastructure.Interfaces;
using REPM.Infrastructure.Persistence;
using REPM.Infrastructure.Repositories;

namespace REPM.Infrastructure;

public static class InfrastructureExtensions
{
    // Inject Services
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        var sqlConnectionString = Environment.GetEnvironmentVariable("DB_CONNECTION_STRING") ??
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
}