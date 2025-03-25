using REPM.API.GraphQL.Mutations;
using REPM.API.GraphQL.Queries;
using REPM.Application;
using REPM.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

builder.Services
    .AddGraphQLServer()
    .AddQueryType<Query>() // Register Query type
    .AddMutationType<Mutation>() // Register Mutation type
    .AddFiltering()
    .AddSorting();

builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddApplicationDependencies(builder.Configuration);


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();




app.Run();

record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}