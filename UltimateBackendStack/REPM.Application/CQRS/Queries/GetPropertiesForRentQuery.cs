using Ardalis.GuardClauses;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using MediatR;
using REPM.Application.DTOs;
using REPM.Application.Filters;
using REPM.Application.Helpers;
using REPM.Domain.Entities;
using REPM.Infrastructure.Interfaces;

namespace REPM.Application.CQRS.Queries;

public record GetPropertiesForRentQuery(PropertyFilters Filters) : IRequest<IQueryable<PropertyDto>>
{
}

public class GetPropertiesForRentQueryHandler : IRequestHandler<GetPropertiesForRentQuery, IQueryable<PropertyDto>>
{
    private readonly IRepository<Property> _propertyRepository;
    private readonly IMapper _mapper;

    public GetPropertiesForRentQueryHandler(IRepository<Property> propertyRepository, IMapper mapper)
    {
        _propertyRepository = propertyRepository;
        _mapper = mapper;
    }

    public async Task<IQueryable<PropertyDto>> Handle(GetPropertiesForRentQuery request, CancellationToken cancellationToken)
    {
        Guard.Against.Null(request, nameof(request));

        // Start Query
        var query = _propertyRepository.QueryToRead
            .Where(p => p.IsListedForRent)
            .AsQueryable();

        // Apply Dynamic Filters
        query = QueryFilterHelper.ApplyFilters(query, request.Filters);

        // Map to DTO
        return query.ProjectTo<PropertyDto>(_mapper.ConfigurationProvider);
    }
}