using MediatR;
using REPM.Application.CQRS.Queries;
using REPM.Application.DTOs;

namespace REPM.API.GraphQL.Queries;

[ExtendObjectType(typeof(Query))]
public class UserQueries
{
    /// <summary>
    /// Get a user by their ID
    /// </summary>
    public async Task<UserDto?> GetUserById(
        Guid id,
        [Service] IMediator mediator)
    {
        var request = new GetUserById(id);
        return await mediator.Send(request);
    }
    
    /// <summary>
    /// Get all users with pagination support
    /// </summary>
    [UsePaging]
    public IQueryable<UserDto> GetUsers(
        [Service] IMediator mediator)
    {
        // For now, return an empty queryable since we don't have a GetAllUsers query implemented
        // You can implement this later when you create a GetAllUsersQuery
        return new List<UserDto>().AsQueryable();
    }
}
