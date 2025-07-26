using Microsoft.EntityFrameworkCore;
using REPM.API.GraphQL.Mutations;
using REPM.API.GraphQL.Queries;
using REPM.Application;
using REPM.Infrastructure;
using REPM.Infrastructure.Persistence;
using REPM.Infrastructure.Persistence.Seeding;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

builder.Services
    .AddGraphQLServer()
    .AddQueryType<Query>() // Register Query type
    .AddMutationType<Mutation>() // Register Mutation type
    .AddTypeExtension<PropertyQueries>()
    .AddTypeExtension<UserQueries>()
    .AddTypeExtension<LeaseQueries>()
    .AddTypeExtension<PropertyMutations>()
    .AddTypeExtension<UserMutations>()
    .AddTypeExtension<LeaseMutations>()
    .AddTypeExtension<PaymentMutations>()
    .AddFiltering()
    .AddSorting();


builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddApplicationDependencies(builder.Configuration);


var app = builder.Build();

// Apply pending migrations automatically in development
if (app.Environment.IsDevelopment())
{
    using (var scope = app.Services.CreateScope())
    {
        var context = scope.ServiceProvider.GetRequiredService<REPMContext>();
        var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
        
        // Apply migrations
        context.Database.Migrate();
        
        // Seed the database
        await DatabaseSeeder.SeedAsync(context, logger);
    }
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.MapGraphQL();

app.Run();