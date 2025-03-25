using Ardalis.GuardClauses;
using AutoMapper;
using MediatR;
using REPM.Application.DTOs;
using REPM.Domain.Entities;
using REPM.Infrastructure.Interfaces;

namespace REPM.Application.CQRS.Commands;

public record UpdatePropertyCommand(PropertyForUpdateDto PropertyForUpdateDto) : IRequest<bool>;

public class UpdatePropertyCommandHandler : IRequestHandler<UpdatePropertyCommand, bool>
{
    private readonly IRepository<Property> _propertyRepository;
    private readonly IMapper _mapper;

    public UpdatePropertyCommandHandler(IRepository<Property> propertyRepository, IMapper mapper)
    {
        _propertyRepository = propertyRepository;
        _mapper = mapper;
    }

    public async Task<bool> Handle(UpdatePropertyCommand request, CancellationToken cancellationToken)
    {
        Guard.Against.Null(request, nameof(request));
        Guard.Against.Null(request.PropertyForUpdateDto, nameof(request.PropertyForUpdateDto));

        var property = await _propertyRepository.GetByIdAsync(request.PropertyForUpdateDto.Id, cancellationToken);
        if (property == null) throw new NotFoundException(nameof(Property), request.PropertyForUpdateDto.Id.ToString());

        _mapper.Map(request.PropertyForUpdateDto, property);
        return await _propertyRepository.SaveChangesAsync(cancellationToken);
    }
}