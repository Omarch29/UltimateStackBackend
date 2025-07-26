using MediatR;
using REPM.Application.CQRS.Queries;
using REPM.Application.DTOs;

namespace REPM.API.GraphQL.Queries;

[ExtendObjectType(typeof(Query))]
public class LeaseQueries
{
    /// <summary>
    /// Get leases by property ID with pagination support
    /// </summary>
    [UsePaging]
    public async Task<IQueryable<LeaseDto>?> GetLeasesByPropertyId(
        Guid propertyId,
        [Service] IMediator mediator)
    {
        var request = new GetLeasesByPropertyIdQuery(propertyId);
        return await mediator.Send(request);
    }
}