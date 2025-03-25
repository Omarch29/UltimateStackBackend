using Ardalis.GuardClauses;
using MediatR;
using Microsoft.EntityFrameworkCore;
using REPM.Domain.Entities;
using REPM.Infrastructure.Interfaces;

namespace REPM.Application.CQRS.Commands;

public record UnlistPropertyForRentCommand(Guid PropertyId) : IRequest<bool>;

public class UnlistPropertyForRentCommandHandler : IRequestHandler<UnlistPropertyForRentCommand, bool>
{
    private readonly IRepository<Property> _propertyRepository;

    public UnlistPropertyForRentCommandHandler(IRepository<Property> propertyRepository)
    {
        _propertyRepository = propertyRepository;
    }

    public async Task<bool> Handle(UnlistPropertyForRentCommand request, CancellationToken cancellationToken)
    {
        Guard.Against.Null(request, nameof(request));
        Guard.Against.Null(request.PropertyId, nameof(request.PropertyId));
        
        var property = await _propertyRepository.Query
            .Include(x => x.Leases)
            .FirstOrDefaultAsync(x => x.Id == request.PropertyId, cancellationToken);
        
        if (property == null) throw new NotFoundException(nameof(Property), request.PropertyId.ToString());
        
        property.UnlistForRent();
        return await _propertyRepository.SaveChangesAsync(cancellationToken);
        
        
    }
}