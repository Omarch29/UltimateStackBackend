using MediatR;
using REPM.Application.CQRS.Commands;
using REPM.Application.DTOs;

namespace REPM.API.GraphQL.Mutations;

[ExtendObjectType(typeof(Mutation))]
public class LeaseMutations
{
    public Task<bool> DeleteLease(Guid id,
        [Service] IMediator mediator)
    {
        return mediator.Send(new DeleteLeaseCommand(id));
    }
    
    public Task<bool> UpdateLease(LeaseForUpdateDto input,
        [Service] IMediator mediator)
    {
        return mediator.Send(new UpdateLeaseCommand(input));
    }
    
    public Task<bool> ExpireLease(Guid id,
        [Service] IMediator mediator)
    {
        return mediator.Send(new ExpireLeaseCommand(id));
    }
    
    public Task<bool> ActivateLease(Guid id,
        [Service] IMediator mediator)
    {
        return mediator.Send(new ActivateLeaseCommand(id));
    }
    
    public Task<bool> CancelLease(Guid id,
        [Service] IMediator mediator)
    {
        return mediator.Send(new CancelLeaseCommand(id));
    }
}