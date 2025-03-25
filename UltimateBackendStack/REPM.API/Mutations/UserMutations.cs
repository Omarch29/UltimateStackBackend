using MediatR;
using REPM.Application.CQRS.Commands;
using REPM.Application.DTOs;

namespace REPM.API.GraphQL.Mutations;

[ExtendObjectType(typeof(Mutation))]
public class UserMutations
{
    public async Task<Guid> CreateUser(
        string firstName, string lastName, string email,
        [Service] IMediator mediator)
    {
        var command = new CreateUserCommand(firstName, lastName, email);
        return await mediator.Send(command);
    }

    public async Task<bool> UpdateUser(
        UserforUpdateDto input,
        [Service] IMediator mediator)
    {
        var command = new UpdateUserCommand(input);
        return await mediator.Send(command);
    }

    public async Task<bool> DeleteUser(
        Guid userId,
        [Service] IMediator mediator)
    {
        var command = new DeleteUserCommand(userId);
        return await mediator.Send(command);
    }
}