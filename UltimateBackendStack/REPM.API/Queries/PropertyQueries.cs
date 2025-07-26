using MediatR;
using REPM.Application.CQRS.Queries;
using REPM.Application.DTOs;
using REPM.Application.Filters;

namespace REPM.API.GraphQL.Queries;

[ExtendObjectType(typeof(Query))]
public class PropertyQueries
{
    /// <summary>
    /// Get properties unlisted by user ID with pagination and filtering support
    /// </summary>
    [UsePaging]
    public async Task<IQueryable<PropertyDto>?> GetProperties(
        Guid userId,
        PropertyFilters? filters,
        [Service] IMediator mediator)
    {
        var query = new GetPropertiesUnlistedByUserIdQuery(userId, filters ?? new PropertyFilters(null, null, null, null, null, null, null, null, null, null, null, null));
        return await mediator.Send(query);
    }
    
    /// <summary>
    /// Get properties for rent with pagination and filtering support
    /// </summary>
    [UsePaging]
    public async Task<IQueryable<PropertyDto>?> GetPropertiesForRent(
        PropertyFilters? filters,
        [Service] IMediator mediator)
    {
        var query = new GetPropertiesForRentQuery(filters ?? new PropertyFilters(null, null, null, null, null, null, null, null, null, null, null, null));
        return await mediator.Send(query);
    }
}