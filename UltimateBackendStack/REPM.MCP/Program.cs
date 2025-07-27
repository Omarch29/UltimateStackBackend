using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using REPM.Application;
using REPM.Infrastructure;
using REPM.Infrastructure.Persistence;
using REPM.MCP.Server;
using REPM.MCP.Handlers;
using REPM.MCP.Models;
using System.Text.Json;

// Configure JSON serializer options for MCP protocol
var jsonOptions = new JsonSerializerOptions
{
    PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
    PropertyNameCaseInsensitive = true,
    WriteIndented = false
};

var builder = Host.CreateApplicationBuilder(args);

// Configure logging - completely disable all logging for MCP
builder.Logging.ClearProviders();
builder.Logging.SetMinimumLevel(LogLevel.None);
// Don't redirect stdout - we need it for JSON-RPC communication

// Add services
builder.Services.AddInfrastructureForMcp(builder.Configuration);
builder.Services.AddApplicationDependencies(builder.Configuration);

// Add MCP services
builder.Services.AddScoped<RealEstateToolHandler>();
builder.Services.AddScoped<McpServer>();

var host = builder.Build();

// Ensure database is created and seeded
try
{
    using (var scope = host.Services.CreateScope())
    {
        var context = scope.ServiceProvider.GetRequiredService<REPMContext>();
        await context.Database.EnsureCreatedAsync();
    }
}
catch (Exception)
{
    // If database connection fails, continue without it
    // The server will still respond to MCP requests but won't have data
}

// Start MCP server
var mcpServer = host.Services.GetRequiredService<McpServer>();
var isInitialized = false;

try
{
    // Read from stdin and write to stdout for MCP protocol
    using var reader = new StreamReader(Console.OpenStandardInput());
    using var writer = new StreamWriter(Console.OpenStandardOutput()) { AutoFlush = true };

    string? line;
    while ((line = await reader.ReadLineAsync()) != null)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(line))
                continue;

            // Parse the JSON-RPC request
            var request = JsonSerializer.Deserialize<McpRequest>(line, jsonOptions);
            if (request == null)
            {
                continue;
            }

            // Handle the request
            using var scope = host.Services.CreateScope();
            var scopedServer = scope.ServiceProvider.GetRequiredService<McpServer>();
            
            // Set initialization state
            if (request.Method == "initialize")
            {
                isInitialized = true;
            }
            scopedServer.SetInitializationState(isInitialized);
            
            var response = await scopedServer.HandleRequest(request);

            // Send the response
            var responseJson = JsonSerializer.Serialize(response, jsonOptions);
            await writer.WriteLineAsync(responseJson);
            await writer.FlushAsync();
        }
        catch (JsonException)
        {
            // Send error response
            var errorResponse = new McpResponse
            {
                Id = "unknown",
                Result = null,
                Error = new McpError
                {
                    Code = -32700,
                    Message = "Parse error"
                }
            };
            
            var errorJson = JsonSerializer.Serialize(errorResponse, jsonOptions);
            await writer.WriteLineAsync(errorJson);
            await writer.FlushAsync();
        }
        catch (Exception)
        {
            // Silent error handling for MCP - don't output anything to stdout
        }
    }
}
catch (Exception)
{
    // Silent error handling for MCP
    Environment.Exit(1);
}
