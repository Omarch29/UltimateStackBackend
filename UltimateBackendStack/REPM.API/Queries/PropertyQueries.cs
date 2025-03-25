using MediatR;
using REPM.Application.CQRS.Queries;
using REPM.Application.DTOs;

namespace REPM.API.GraphQL.Queries;

[ExtendObjectType(typeof(Query))]
public class PropertyQueries
{
    [UsePaging] // Adds pagination automatically
    public async Task<IQueryable<PropertyDto>?> GetProperties(
        GetPropertiesUnlistedByUserIdQueryHandler request,
        [Service] IMediator mediator)
    {
        return await mediator.Send(request) as IQueryable<PropertyDto>;
    }
    
    
    [UsePaging] // Adds pagination automatically
    public async Task<IQueryable<PropertyDto>?> GetPropertiesForRent(
        GetPropertiesForRentQueryHandler request,
        [Service] IMediator mediator)
    {
        return await mediator.Send(request) as IQueryable<PropertyDto>;
    }
    
    
}