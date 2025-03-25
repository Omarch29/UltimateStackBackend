using MediatR;
using REPM.Application.CQRS.Commands;
using REPM.Application.DTOs;

namespace REPM.API.GraphQL.Mutations;

[ExtendObjectType(typeof(Mutation))]
public class PaymentMutations
{
 
    public async Task<Guid> MakePayment(
        PaymentForCreateDto input,
        [Service] IMediator mediator)
    {
        var command = new MakePaymentCommand(input);
        return await mediator.Send(command);
    }
    
    public async Task<bool> UpdatePayment(
        PaymentForUpdateDto input,
        [Service] IMediator mediator)
    {
        var command = new UpdatePaymentCommand(input);
        return await mediator.Send(command);
    }
    
    public async Task<bool> DeletePayment(
        Guid id,
        [Service] IMediator mediator)
    {
        var command = new DeletePaymentCommand(id);
        return await mediator.Send(command);
    }
    
    public async Task<bool> CompletePayment(
        Guid id,
        [Service] IMediator mediator)
    {
        var command = new CompletePaymentCommand(id);
        return await mediator.Send(command);
    }
    
    public async Task<bool> CancelPayment(
        Guid id,
        [Service] IMediator mediator)
    {
        var command = new CancelPaymentCommand(id);
        return await mediator.Send(command);
    }
}