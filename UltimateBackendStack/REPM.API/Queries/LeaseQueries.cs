using MediatR;
using REPM.Application.CQRS.Queries;
using REPM.Application.DTOs;

namespace REPM.API.GraphQL.Queries;

[ExtendObjectType(typeof(Query))]
public class LeaseQueries
{
    [UsePaging] // Adds pagination automatically
    public async Task<IQueryable<LeaseDto>?> GetLeases(
        GetLeasesByPropertyIdQuery request,
        [Service] IMediator mediator)
    {
        return await mediator.Send(request) as IQueryable<LeaseDto>;
    }
    
}