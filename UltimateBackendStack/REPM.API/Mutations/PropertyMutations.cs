using MediatR;
using REPM.Application.CQRS.Commands;
using REPM.Application.DTOs;

namespace REPM.API.GraphQL.Mutations;

[ExtendObjectType(typeof(Mutation))]
public class PropertyMutations
{
    public async Task<Guid> CreateProperty(
        CreatePropetyInput input,
        [Service] IMediator mediator)
    {
        var command = new CreatePropertyCommand(input.Name, input.Address, input.Description, input.Price, input.Beds, input.Baths, input.SquareFeet, input.IsActive, input.OwnerId);
        return await mediator.Send(command);
    }
    
    public async Task<bool> UpdateProperty(
        PropertyForUpdateDto input,
        [Service] IMediator mediator)
    {
        var command = new UpdatePropertyCommand(input);
        return await mediator.Send(command);
    }
    
    public async Task<bool> DeleteProperty(
        Guid propertyId,
        [Service] IMediator mediator)
    {
        var command = new DeletePropertyCommand(propertyId);
        return await mediator.Send(command);
    }
    
    public async Task<Guid> LeaseProperty(
        LeasePropertyInput input,
        [Service] IMediator mediator)
    {
        var command = new LeasePropertyCommand(input.PropertyId, input.TenantId, input.DateRange, input.Price);
        return await mediator.Send(command);
    }
    
    public async Task<bool> ListPropertyForRent(
        Guid propertyId,
        [Service] IMediator mediator)
    {
        var command = new ListPropertyForRentCommand(propertyId);
        return await mediator.Send(command);
    }
    
    public async Task<bool> UnlistPropertyForRent(
        Guid propertyId,
        [Service] IMediator mediator)
    {
        var command = new UnlistPropertyForRentCommand(propertyId);
        return await mediator.Send(command);
    }
    
    
}


public record CreatePropetyInput(string Name, AddressDto Address, string Description, decimal Price, int Beds, int Baths, int SquareFeet, bool IsActive, Guid OwnerId);

public record LeasePropertyInput(Guid PropertyId, Guid TenantId, DateRangeDto DateRange, MoneyDto Price);