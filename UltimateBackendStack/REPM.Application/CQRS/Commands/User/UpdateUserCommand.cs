using Ardalis.GuardClauses;
using AutoMapper;
using MediatR;
using REPM.Application.DTOs;
using REPM.Domain.Entities;
using REPM.Infrastructure.Interfaces;

namespace REPM.Application.CQRS.Commands;

public record UpdateUserCommand(UserforUpdateDto UserToUpdate) : IRequest<bool>;

public class UpdateUserCommandHandler : IRequestHandler<UpdateUserCommand, bool>
{
    private readonly IRepository<User> _userRepository;
    private readonly IMapper _mapper;

    public UpdateUserCommandHandler(IRepository<User> userRepository, IMapper mapper)
    {
        _userRepository = userRepository;
        _mapper = mapper;
    }

    public async Task<bool> Handle(UpdateUserCommand request, CancellationToken cancellationToken)
    {
        Guard.Against.Null(request, nameof(request));
        Guard.Against.Null(request.UserToUpdate, nameof(request.UserToUpdate));

        var user = await _userRepository.GetByIdAsync(request.UserToUpdate.Id, cancellationToken);
        if (user == null) throw new NotFoundException(nameof(User), request.UserToUpdate.Id.ToString());
        
        _mapper.Map(request.UserToUpdate, user);
        return await _userRepository.SaveChangesAsync(cancellationToken);

    }
}

