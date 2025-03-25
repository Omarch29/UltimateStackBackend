using Ardalis.GuardClauses;
using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using REPM.Application.DTOs;
using REPM.Domain.Entities;
using REPM.Infrastructure.Interfaces;

namespace REPM.Application.CQRS.Queries;

public record GetPropertyByIdQuery(Guid Id) : IRequest<PropertyWithLeasesDto>
{
}

public class GetPropertyByIdQueryHandler : IRequestHandler<GetPropertyByIdQuery, PropertyWithLeasesDto>
{
    private readonly IMapper _mapper;
    private readonly IRepository<Property> _propertyRepository;
    
    public GetPropertyByIdQueryHandler(IMapper mapper, IRepository<Property> propertyRepository)
    {
        _mapper = mapper;
        _propertyRepository = propertyRepository;
    }
    
    public async Task<PropertyWithLeasesDto> Handle(GetPropertyByIdQuery request, CancellationToken cancellationToken)
    {
        Guard.Against.Null(request, nameof(request));
        Guard.Against.Null(request.Id, nameof(request.Id));
        
        var property = await _propertyRepository.QueryToRead
            .Include(p => p.Leases)
            .FirstOrDefaultAsync(p => p.Id == request.Id, cancellationToken: cancellationToken);
        
        if (property != null)
        {
            return _mapper.Map<PropertyWithLeasesDto>(property);
        }
        
        throw new NotFoundException(nameof(Property), request.Id.ToString());
    }
}