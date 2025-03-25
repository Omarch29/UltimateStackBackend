using Ardalis.GuardClauses;
using MediatR;
using REPM.Domain.Entities;
using REPM.Infrastructure.Interfaces;

namespace REPM.Application.CQRS.Commands;

public record DeleteUserCommand(Guid Id) : IRequest<bool>;

public class DeleteCommandHandler : IRequestHandler<DeleteUserCommand, bool>
{
    private readonly IRepository<User> _userRepository;

    public DeleteCommandHandler(IRepository<User> userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<bool> Handle(DeleteUserCommand request, CancellationToken cancellationToken)
    {
        Guard.Against.Null(request, nameof(request));
        Guard.Against.Null(request.Id, nameof(request.Id));

        var user = await _userRepository.GetByIdAsync(request.Id);
        if (user == null) throw new NotFoundException(nameof(User), request.Id.ToString());
        
        _userRepository.Delete(user);
        await _userRepository.SaveChangesAsync(cancellationToken);
        return true;

    }
}