using Ardalis.GuardClauses;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using REPM.Application.DTOs;
using REPM.Domain.Entities;
using REPM.Infrastructure.Interfaces;

namespace REPM.Application.CQRS.Queries;

public record GetLeasesByPropertyIdQuery(Guid PropertyId): IRequest<IQueryable<LeaseDto>>
{
    
}

public class GetLeasesByPropertyIdQueryHandler : IRequestHandler<GetLeasesByPropertyIdQuery, IQueryable<LeaseDto>>
{
    private readonly IMapper _mapper;
    private readonly IRepository<Lease> _leaseRepository;
    
    public GetLeasesByPropertyIdQueryHandler(IMapper mapper, IRepository<Lease> leaseRepository)
    {
        _mapper = mapper;
        _leaseRepository = leaseRepository;
    }
    
    public async Task<IQueryable<LeaseDto>> Handle(GetLeasesByPropertyIdQuery request, CancellationToken cancellationToken)
    {
        Guard.Against.Null(request, nameof(request));
        Guard.Against.Null(request.PropertyId, nameof(request.PropertyId));
        
        var leases =  _leaseRepository.QueryToRead
            .Include(l => l.Payments)
            .Include(l => l.Tenant)
            .Include(l => l.Property)
            .Where(l => l.PropertyId == request.PropertyId)
            .AsQueryable();
        

        return leases.ProjectTo<LeaseDto>(_mapper.ConfigurationProvider);
    }
}