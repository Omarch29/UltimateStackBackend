using MediatR;
using REPM.Application.CQRS.Queries;
using REPM.Application.DTOs;

namespace REPM.API.GraphQL.Queries;


[ExtendObjectType(typeof(Query))]
public class UserQueries
{
    [UsePaging] // Adds pagination automatically
    public async Task<UserDto> GetUsers(
        GetUserById request,
        [Service] IMediator mediator)
    {
        return await mediator.Send(request);
    }
}
