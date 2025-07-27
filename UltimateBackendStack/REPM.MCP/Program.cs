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

var builder = Host.CreateApplicationBuilder(args);

// Configure logging
builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Logging.SetMinimumLevel(LogLevel.Warning); // Reduce noise for MCP

// Add services
builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddApplicationDependencies(builder.Configuration);

// Add MCP services
builder.Services.AddScoped<RealEstateToolHandler>();
builder.Services.AddScoped<McpServer>();

var host = builder.Build();

// Ensure database is created and seeded
using (var scope = host.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<REPMContext>();
    await context.Database.EnsureCreatedAsync();
}

// Start MCP server
var logger = host.Services.GetRequiredService<ILogger<Program>>();
var mcpServer = host.Services.GetRequiredService<McpServer>();

logger.LogInformation("REPM MCP Server starting...");

try
{
    // Read from stdin and write to stdout for MCP protocol
    using var reader = new StreamReader(Console.OpenStandardInput());
    using var writer = new StreamWriter(Console.OpenStandardOutput());

    string? line;
    while ((line = await reader.ReadLineAsync()) != null)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(line))
                continue;

            // Parse the JSON-RPC request
            var request = JsonSerializer.Deserialize<McpRequest>(line);
            if (request == null)
            {
                logger.LogWarning("Received null request");
                continue;
            }

            // Handle the request
            using var scope = host.Services.CreateScope();
            var scopedServer = scope.ServiceProvider.GetRequiredService<McpServer>();
            var response = await scopedServer.HandleRequest(request);

            // Send the response
            var responseJson = JsonSerializer.Serialize(response);
            await writer.WriteLineAsync(responseJson);
            await writer.FlushAsync();
        }
        catch (JsonException ex)
        {
            logger.LogError(ex, "Invalid JSON received: {Line}", line);
            
            // Send error response
            var errorResponse = new McpResponse
            {
                Id = "unknown",
                Error = new McpError
                {
                    Code = -32700,
                    Message = "Parse error"
                }
            };
            
            var errorJson = JsonSerializer.Serialize(errorResponse);
            await writer.WriteLineAsync(errorJson);
            await writer.FlushAsync();
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Unexpected error processing request");
        }
    }
}
catch (Exception ex)
{
    logger.LogError(ex, "Fatal error in MCP server");
    throw;
}

logger.LogInformation("REPM MCP Server stopped.");
