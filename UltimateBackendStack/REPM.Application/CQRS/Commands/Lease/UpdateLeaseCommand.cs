using Ardalis.GuardClauses;
using AutoMapper;
using MediatR;
using REPM.Application.DTOs;
using REPM.Domain.Entities;
using REPM.Infrastructure.Interfaces;

namespace REPM.Application.CQRS.Commands;

public record UpdateLeaseCommand(LeaseForUpdateDto LeaseForUpdateDto) : IRequest<bool>;

public class UpdateLeaseCommandHandler : IRequestHandler<UpdateLeaseCommand, bool>
{
    private readonly IRepository<Lease> _leaseRepository;
    private readonly IMapper _mapper;

    public UpdateLeaseCommandHandler(IRepository<Lease> leaseRepository, IMapper mapper)
    {
        _leaseRepository = leaseRepository;
        _mapper = mapper;
    }

    public Task<bool> Handle(UpdateLeaseCommand request, CancellationToken cancellationToken)
    {
        Guard.Against.Null(request, nameof(request));
        Guard.Against.Null(request.LeaseForUpdateDto, nameof(request.LeaseForUpdateDto));
        
        var lease = _leaseRepository.GetByIdAsync(request.LeaseForUpdateDto.Id, cancellationToken).Result;
        if (lease == null) throw new NotFoundException(nameof(Lease), request.LeaseForUpdateDto.Id.ToString());
        
        _mapper.Map(request.LeaseForUpdateDto, lease);
        return _leaseRepository.SaveChangesAsync(cancellationToken);
    }
}