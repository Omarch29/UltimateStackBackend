using MediatR;
using REPM.Application.CQRS.Queries;
using REPM.Application.CQRS.Commands;
using REPM.Application.DTOs;
using REPM.Application.Filters;
using REPM.MCP.Models;
using System.Text.Json;
using Microsoft.Extensions.Logging;

namespace REPM.MCP.Handlers;

public class RealEstateToolHandler
{
    private readonly IMediator _mediator;
    private readonly ILogger<RealEstateToolHandler> _logger;

    public RealEstateToolHandler(IMediator mediator, ILogger<RealEstateToolHandler> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    public async Task<ToolResult> HandleToolCall(ToolCall toolCall)
    {
        try
        {
            return toolCall.Name switch
            {
                "list_properties_for_rent" => await HandleListPropertiesForRent(toolCall.Arguments),
                "get_property_details" => await HandleGetPropertyDetails(toolCall.Arguments),
                "get_users" => await HandleGetUsers(toolCall.Arguments),
                "get_user_properties" => await HandleGetUserProperties(toolCall.Arguments),
                "get_leases_by_property" => await HandleGetLeasesByProperty(toolCall.Arguments),
                "create_lease" => await HandleCreateLease(toolCall.Arguments),
                "make_payment" => await HandleMakePayment(toolCall.Arguments),
                "search_properties" => await HandleSearchProperties(toolCall.Arguments),
                _ => CreateErrorResult($"Unknown tool: {toolCall.Name}")
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error handling tool call {ToolName}", toolCall.Name);
            return CreateErrorResult($"Error executing tool {toolCall.Name}: {ex.Message}");
        }
    }

    private async Task<ToolResult> HandleListPropertiesForRent(Dictionary<string, object> arguments)
    {
        var filters = CreatePropertyFilters(arguments);
        var query = new GetPropertiesForRentQuery(filters);
        var properties = await _mediator.Send(query);

        var propertiesList = properties?.ToList() ?? new List<PropertyDto>();
        var result = JsonSerializer.Serialize(propertiesList, new JsonSerializerOptions { WriteIndented = true });

        return new ToolResult
        {
            Content = new List<ContentItem>
            {
                new() { Type = "text", Text = $"Found {propertiesList.Count} properties available for rent:\n{result}" }
            }
        };
    }

    private async Task<ToolResult> HandleGetPropertyDetails(Dictionary<string, object> arguments)
    {
        if (!arguments.TryGetValue("propertyId", out var propertyIdObj) || 
            !Guid.TryParse(propertyIdObj.ToString(), out var propertyId))
        {
            return CreateErrorResult("Invalid or missing propertyId");
        }

        // For now, we'll get all properties and filter by ID
        // In a real implementation, you might want a specific GetPropertyByIdQuery
        var query = new GetPropertiesForRentQuery(new PropertyFilters(null, null, null, null, null, null, null, null, null, null, null, null));
        var properties = await _mediator.Send(query);
        var property = properties?.FirstOrDefault(p => p.Id == propertyId);

        if (property == null)
        {
            return CreateErrorResult($"Property with ID {propertyId} not found");
        }

        var result = JsonSerializer.Serialize(property, new JsonSerializerOptions { WriteIndented = true });

        return new ToolResult
        {
            Content = new List<ContentItem>
            {
                new() { Type = "text", Text = $"Property Details:\n{result}" }
            }
        };
    }

    private async Task<ToolResult> HandleGetUsers(Dictionary<string, object> arguments)
    {
        var query = new GetUsersQuery();
        var users = await _mediator.Send(query);

        var usersList = users?.ToList() ?? new List<UserDto>();
        var result = JsonSerializer.Serialize(usersList, new JsonSerializerOptions { WriteIndented = true });

        return new ToolResult
        {
            Content = new List<ContentItem>
            {
                new() { Type = "text", Text = $"Found {usersList.Count} users:\n{result}" }
            }
        };
    }

    private async Task<ToolResult> HandleGetUserProperties(Dictionary<string, object> arguments)
    {
        if (!arguments.TryGetValue("userId", out var userIdObj) || 
            !Guid.TryParse(userIdObj.ToString(), out var userId))
        {
            return CreateErrorResult("Invalid or missing userId");
        }

        var query = new GetPropertiesUnlistedByUserIdQuery(userId, new PropertyFilters(null, null, null, null, null, null, null, null, null, null, null, null));
        var properties = await _mediator.Send(query);

        var propertiesList = properties?.ToList() ?? new List<PropertyDto>();
        var result = JsonSerializer.Serialize(propertiesList, new JsonSerializerOptions { WriteIndented = true });

        return new ToolResult
        {
            Content = new List<ContentItem>
            {
                new() { Type = "text", Text = $"User {userId} owns {propertiesList.Count} properties:\n{result}" }
            }
        };
    }

    private async Task<ToolResult> HandleGetLeasesByProperty(Dictionary<string, object> arguments)
    {
        if (!arguments.TryGetValue("propertyId", out var propertyIdObj) || 
            !Guid.TryParse(propertyIdObj.ToString(), out var propertyId))
        {
            return CreateErrorResult("Invalid or missing propertyId");
        }

        var query = new GetLeasesByPropertyIdQuery(propertyId);
        var leases = await _mediator.Send(query);

        var leasesList = leases?.ToList() ?? new List<LeaseDto>();
        var result = JsonSerializer.Serialize(leasesList, new JsonSerializerOptions { WriteIndented = true });

        return new ToolResult
        {
            Content = new List<ContentItem>
            {
                new() { Type = "text", Text = $"Property {propertyId} has {leasesList.Count} leases:\n{result}" }
            }
        };
    }

    private async Task<ToolResult> HandleCreateLease(Dictionary<string, object> arguments)
    {
        try
        {
            if (!Guid.TryParse(arguments["propertyId"].ToString(), out var propertyId))
                return CreateErrorResult("Invalid propertyId");

            if (!Guid.TryParse(arguments["tenantId"].ToString(), out var tenantId))
                return CreateErrorResult("Invalid tenantId");

            if (!DateTime.TryParse(arguments["startDate"].ToString(), out var startDate))
                return CreateErrorResult("Invalid startDate");

            if (!DateTime.TryParse(arguments["endDate"].ToString(), out var endDate))
                return CreateErrorResult("Invalid endDate");

            if (!decimal.TryParse(arguments["monthlyRent"].ToString(), out var monthlyRent))
                return CreateErrorResult("Invalid monthlyRent");

            var currency = arguments.GetValueOrDefault("currency")?.ToString() ?? "USD";

            var dateRange = new DateRangeDto(startDate, endDate);
            var money = new MoneyDto(monthlyRent, currency);

            var command = new LeasePropertyCommand(propertyId, tenantId, dateRange, money);
            var leaseId = await _mediator.Send(command);

            return new ToolResult
            {
                Content = new List<ContentItem>
                {
                    new() { Type = "text", Text = $"Lease created successfully with ID: {leaseId}" }
                }
            };
        }
        catch (Exception ex)
        {
            return CreateErrorResult($"Error creating lease: {ex.Message}");
        }
    }

    private async Task<ToolResult> HandleMakePayment(Dictionary<string, object> arguments)
    {
        try
        {
            if (!Guid.TryParse(arguments["leaseId"].ToString(), out var leaseId))
                return CreateErrorResult("Invalid leaseId");

            if (!decimal.TryParse(arguments["amount"].ToString(), out var amount))
                return CreateErrorResult("Invalid amount");

            if (!DateTime.TryParse(arguments["paymentDate"].ToString(), out var paymentDate))
                return CreateErrorResult("Invalid paymentDate");

            var currency = arguments.GetValueOrDefault("currency")?.ToString() ?? "USD";
            var description = arguments.GetValueOrDefault("description")?.ToString() ?? "Monthly rent payment";

            var money = new MoneyDto(amount, currency);
            var paymentDto = new PaymentForCreateDto(leaseId, money, paymentDate);
            var command = new MakePaymentCommand(paymentDto);
            
            var paymentId = await _mediator.Send(command);

            return new ToolResult
            {
                Content = new List<ContentItem>
                {
                    new() { Type = "text", Text = $"Payment recorded successfully with ID: {paymentId}" }
                }
            };
        }
        catch (Exception ex)
        {
            return CreateErrorResult($"Error making payment: {ex.Message}");
        }
    }

    private async Task<ToolResult> HandleSearchProperties(Dictionary<string, object> arguments)
    {
        var filters = CreatePropertyFilters(arguments);
        var query = new GetPropertiesForRentQuery(filters);
        var properties = await _mediator.Send(query);

        var propertiesList = properties?.ToList() ?? new List<PropertyDto>();
        var result = JsonSerializer.Serialize(propertiesList, new JsonSerializerOptions { WriteIndented = true });

        return new ToolResult
        {
            Content = new List<ContentItem>
            {
                new() { Type = "text", Text = $"Search found {propertiesList.Count} properties:\n{result}" }
            }
        };
    }

    private PropertyFilters CreatePropertyFilters(Dictionary<string, object> arguments)
    {
        var city = arguments.GetValueOrDefault("city")?.ToString();
        var state = arguments.GetValueOrDefault("state")?.ToString();
        var propertyType = arguments.GetValueOrDefault("propertyType")?.ToString();

        decimal? minPrice = null, maxPrice = null;
        int? minBedrooms = null, maxBedrooms = null, bedrooms = null;

        if (arguments.TryGetValue("minPrice", out var minPriceObj) && decimal.TryParse(minPriceObj.ToString(), out var minPriceVal))
            minPrice = minPriceVal;

        if (arguments.TryGetValue("maxPrice", out var maxPriceObj) && decimal.TryParse(maxPriceObj.ToString(), out var maxPriceVal))
            maxPrice = maxPriceVal;

        if (arguments.TryGetValue("minBedrooms", out var minBedroomsObj) && int.TryParse(minBedroomsObj.ToString(), out var minBedroomsVal))
            minBedrooms = minBedroomsVal;

        if (arguments.TryGetValue("maxBedrooms", out var maxBedroomsObj) && int.TryParse(maxBedroomsObj.ToString(), out var maxBedroomsVal))
            maxBedrooms = maxBedroomsVal;

        if (arguments.TryGetValue("bedrooms", out var bedroomsObj) && int.TryParse(bedroomsObj.ToString(), out var bedroomsVal))
            bedrooms = bedroomsVal;

        MoneyDto? minPriceDto = minPrice.HasValue ? new MoneyDto(minPrice.Value, "USD") : null;
        MoneyDto? maxPriceDto = maxPrice.HasValue ? new MoneyDto(maxPrice.Value, "USD") : null;

        return new PropertyFilters(
            null, city, state, null, minPrice, maxPrice,
            minBedrooms, maxBedrooms, null, null, null, null);
    }

    private ToolResult CreateErrorResult(string message)
    {
        return new ToolResult
        {
            IsError = true,
            Content = new List<ContentItem>
            {
                new() { Type = "text", Text = $"Error: {message}" }
            }
        };
    }
}
