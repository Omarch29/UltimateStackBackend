using Ardalis.GuardClauses;
using MediatR;
using Microsoft.EntityFrameworkCore;
using REPM.Domain.Entities;
using REPM.Infrastructure.Interfaces;

namespace REPM.Application.CQRS.Commands;

public record ListPropertyForRentCommand(Guid PropertyId) : IRequest<bool>;

public class ListPropertyForRentCommandHandler : IRequestHandler<ListPropertyForRentCommand, bool>
{
    private readonly IRepository<Property> _propertyRepository;

    public ListPropertyForRentCommandHandler(IRepository<Property> propertyRepository)
    {
        _propertyRepository = propertyRepository;
    }

    public async Task<bool> Handle(ListPropertyForRentCommand request, CancellationToken cancellationToken)
    {
        Guard.Against.Null(request, nameof(request));
        Guard.Against.Null(request.PropertyId, nameof(request.PropertyId));
        
        var property = await _propertyRepository.Query
            .Include(x => x.Leases)
            .FirstOrDefaultAsync(x => x.Id == request.PropertyId, cancellationToken);
        
        if (property == null) throw new NotFoundException(nameof(Property), request.PropertyId.ToString());
        
        property.ListForRent();
        return await _propertyRepository.SaveChangesAsync(cancellationToken);
        
        
    }
}