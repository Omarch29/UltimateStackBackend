using REPM.MCP.Models;
using REPM.MCP.Tools;
using REPM.MCP.Handlers;
using System.Text.Json;

namespace REPM.MCP.Server;

public class McpServer
{
    private readonly RealEstateToolHandler _toolHandler;
    private bool _initialized = false;
    private readonly JsonSerializerOptions _jsonOptions;

    public McpServer(RealEstateToolHandler toolHandler)
    {
        _toolHandler = toolHandler;
        _jsonOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            PropertyNameCaseInsensitive = true,
            WriteIndented = false
        };
    }

    public void SetInitializationState(bool initialized)
    {
        _initialized = initialized;
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
            return CreateErrorResponse(request.Id, -32603, $"Internal error: {ex.Message}");
        }
    }

    private Task<McpResponse> HandleInitialize(McpRequest request)
    {
        try
        {
            var initParams = JsonSerializer.Deserialize<InitializeParams>(
                JsonSerializer.Serialize(request.Params, _jsonOptions), _jsonOptions);

            _initialized = true;

            var result = new InitializeResult
            {
                ProtocolVersion = "2025-06-18",
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

            return Task.FromResult(new McpResponse
            {
                Id = request.Id,
                Result = result,
                Error = null // Explicitly set to null so it's excluded
            });
        }
        catch (Exception)
        {
            return Task.FromResult(CreateErrorResponse(request.Id, -32602, "Invalid initialization parameters"));
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
            Result = result,
            Error = null // Explicitly set to null so it's excluded
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
                JsonSerializer.Serialize(request.Params, _jsonOptions), _jsonOptions);

            if (toolCall == null)
            {
                return CreateErrorResponse(request.Id, -32602, "Invalid tool call parameters");
            }

            var result = await _toolHandler.HandleToolCall(toolCall);

            return new McpResponse
            {
                Id = request.Id,
                Result = result,
                Error = null // Explicitly set to null so it's excluded
            };
        }
        catch (Exception ex)
        {
            return CreateErrorResponse(request.Id, -32603, $"Tool execution error: {ex.Message}");
        }
    }

    private McpResponse CreateErrorResponse(object id, int code, string message)
    {
        return new McpResponse
        {
            Id = id,
            Result = null, // Explicitly set to null so it's excluded
            Error = new McpError
            {
                Code = code,
                Message = message
            }
        };
    }
}
