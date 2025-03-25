using Ardalis.GuardClauses;
using AutoMapper;
using MediatR;
using REPM.Application.DTOs;
using REPM.Domain.Entities;
using REPM.Domain.ValueObjects;
using REPM.Infrastructure.Interfaces;

namespace REPM.Application.CQRS.Commands;

public record CreatePropertyCommand(string Name, AddressDto Address, string Description, decimal Price, int Beds, int Baths, int SquareFeet, bool IsActive, Guid OwnerId) : IRequest<Guid>;



public class CreatePropertyCommandHandler: IRequestHandler<CreatePropertyCommand, Guid>
{
    private readonly IRepository<Property> _propertyRepository;
    private readonly IMapper _mapper;

    public CreatePropertyCommandHandler(IRepository<Property> propertyRepository, IMapper mapper)
    {
        _propertyRepository = propertyRepository;
        _mapper = mapper;
    }

    public async Task<Guid> Handle(CreatePropertyCommand request, CancellationToken cancellationToken)
    {
        Guard.Against.Null(request, nameof(request));
        Guard.Against.NullOrEmpty(request.Name, nameof(request.Name));
        Guard.Against.Null(request.Address, nameof(request.Address));
        Guard.Against.NullOrEmpty(request.Description, nameof(request.Description));
        Guard.Against.NegativeOrZero(request.Price, nameof(request.Price));
        Guard.Against.NegativeOrZero(request.Beds, nameof(request.Beds));
        Guard.Against.NegativeOrZero(request.Baths, nameof(request.Baths));
        Guard.Against.NegativeOrZero(request.SquareFeet, nameof(request.SquareFeet));
        Guard.Against.Null(request.OwnerId, nameof(request.OwnerId));
        
        var property = new Property(request.Name,
            _mapper.Map<Address>(request.Address), 
            request.OwnerId,
            request.Description, request.Price, request.Beds, request.Baths, request.SquareFeet);
        
        _propertyRepository.Insert(property);
        await _propertyRepository.SaveChangesAsync(cancellationToken);
        
        return property.Id;
    }
}