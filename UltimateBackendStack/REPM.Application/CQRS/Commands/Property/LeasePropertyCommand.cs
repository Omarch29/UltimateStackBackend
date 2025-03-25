using Ardalis.GuardClauses;
using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using REPM.Application.DTOs;
using REPM.Domain.DomainServices;
using REPM.Domain.Entities;
using REPM.Domain.ValueObjects;
using REPM.Infrastructure.Interfaces;

namespace REPM.Application.CQRS.Commands;

public record LeasePropertyCommand(Guid PropertyId, Guid TenantId, DateRangeDto DateRange, MoneyDto Price) : IRequest<Guid>;

public class LeasePropertyCommandHandler : IRequestHandler<LeasePropertyCommand, Guid>
{
    private readonly IRepository<Lease> _leaseRepository;
    private readonly IRepository<User> _userRepository;
    private readonly IRepository<Property> _propertyRepository;
    private readonly IMapper _mapper;

    public LeasePropertyCommandHandler(IRepository<Lease> leaseRepository, IMapper mapper, IRepository<User> userRepository, IRepository<Property> propertyRepository)
    {
        _leaseRepository = leaseRepository;
        _mapper = mapper;
        _userRepository = userRepository;
        _propertyRepository = propertyRepository;
    }

    public async Task<Guid> Handle(LeasePropertyCommand request, CancellationToken cancellationToken)
    {
        Guard.Against.Null(request, nameof(request));
        Guard.Against.Null(request.PropertyId, nameof(request.PropertyId));
        Guard.Against.Null(request.TenantId, nameof(request.TenantId));
        Guard.Against.Null(request.DateRange, nameof(request.DateRange));
        Guard.Against.Null(request.Price, nameof(request.Price));
        
        // Get the property
        var property = await _propertyRepository.GetByIdReadOnlyAsync(request.PropertyId, cancellationToken);
        
        // Get the renter
        var tenant = await _userRepository.GetByIdReadOnlyAsync(request.TenantId, cancellationToken);
        
        // Get the Active Leases from the renter including payments
        var renterLeases = await _leaseRepository.QueryToRead
            .Include(x => x.Payments)
            .Where(x => x.TenantId == request.TenantId)
            .ToListAsync(cancellationToken);
        
        var newLease = LeaseDomainService.CreateLease(tenant, 
            property, 
            _mapper.Map<DateRange>(request.DateRange), 
            _mapper.Map<Money>(request.Price), 
            renterLeases);
        
        _leaseRepository.Insert(newLease);
        await _leaseRepository.SaveChangesAsync(cancellationToken);
        return newLease.Id;
    }
}