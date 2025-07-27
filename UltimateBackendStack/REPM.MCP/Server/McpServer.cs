using Microsoft.Extensions.Logging;
using REPM.MCP.Models;
using REPM.MCP.Tools;
using REPM.MCP.Handlers;
using System.Text.Json;

namespace REPM.MCP.Server;

public class McpServer
{
    private readonly RealEstateToolHandler _toolHandler;
    private readonly ILogger<McpServer> _logger;
    private bool _initialized = false;

    public McpServer(RealEstateToolHandler toolHandler, ILogger<McpServer> logger)
    {
        _toolHandler = toolHandler;
        _logger = logger;
    }

    public async Task<McpResponse> HandleRequest(McpRequest request)
    {
        try
        {
            return request.Method switch
            {
                "initialize" => await HandleInitialize(request),
                "tools/list" => HandleListTools(request),
                "tools/call" => await HandleToolCall(request),
                _ => CreateErrorResponse(request.Id, -32601, $"Method not found: {request.Method}")
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error handling MCP request {Method}", request.Method);
            return CreateErrorResponse(request.Id, -32603, $"Internal error: {ex.Message}");
        }
    }

    private async Task<McpResponse> HandleInitialize(McpRequest request)
    {
        try
        {
            var initParams = JsonSerializer.Deserialize<InitializeParams>(
                JsonSerializer.Serialize(request.Params));

            _initialized = true;

            var result = new InitializeResult
            {
                ProtocolVersion = "2024-11-05",
                ServerInfo = new ServerInfo
                {
                    Name = "REPM MCP Server",
                    Version = "1.0.0"
                },
                Capabilities = new ServerCapabilities
                {
                    Tools = new { }
                }
            };

            _logger.LogInformation("MCP Server initialized with protocol version {Version}", 
                initParams?.ProtocolVersion);

            return new McpResponse
            {
                Id = request.Id,
                Result = result
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during initialization");
            return CreateErrorResponse(request.Id, -32602, "Invalid initialization parameters");
        }
    }

    private McpResponse HandleListTools(McpRequest request)
    {
        if (!_initialized)
        {
            return CreateErrorResponse(request.Id, -32002, "Server not initialized");
        }

        var tools = RealEstateTools.GetAllTools();

        var result = new
        {
            tools = tools
        };

        return new McpResponse
        {
            Id = request.Id,
            Result = result
        };
    }

    private async Task<McpResponse> HandleToolCall(McpRequest request)
    {
        if (!_initialized)
        {
            return CreateErrorResponse(request.Id, -32002, "Server not initialized");
        }

        try
        {
            var toolCall = JsonSerializer.Deserialize<ToolCall>(
                JsonSerializer.Serialize(request.Params));

            if (toolCall == null)
            {
                return CreateErrorResponse(request.Id, -32602, "Invalid tool call parameters");
            }

            var result = await _toolHandler.HandleToolCall(toolCall);

            return new McpResponse
            {
                Id = request.Id,
                Result = result
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error handling tool call");
            return CreateErrorResponse(request.Id, -32603, $"Tool execution error: {ex.Message}");
        }
    }

    private McpResponse CreateErrorResponse(string id, int code, string message)
    {
        return new McpResponse
        {
            Id = id,
            Error = new McpError
            {
                Code = code,
                Message = message
            }
        };
    }
}
