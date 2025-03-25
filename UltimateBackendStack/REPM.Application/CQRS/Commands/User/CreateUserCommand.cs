using Ardalis.GuardClauses;
using AutoMapper;
using MediatR;
using REPM.Domain.Entities;
using REPM.Infrastructure.Interfaces;

namespace REPM.Application.CQRS.Commands;

public record CreateUserCommand(string FirstName, string LastName, string Email) : IRequest<Guid>;

public class CreateUserCommandHandler : IRequestHandler<CreateUserCommand, Guid>
{
    private readonly IRepository<User> _userRepository;


    public CreateUserCommandHandler( IRepository<User> userRepository)
    {

        _userRepository = userRepository;

    }

    public async Task<Guid> Handle(CreateUserCommand request, CancellationToken cancellationToken)
    {
        Guard.Against.Null(request, nameof(request));
        Guard.Against.NullOrEmpty(request.FirstName, nameof(request.FirstName));
        Guard.Against.NullOrEmpty(request.LastName, nameof(request.LastName));
        Guard.Against.NullOrEmpty(request.Email, nameof(request.Email));

        var user = new User(string.Concat(request.FirstName, " ", request.LastName), request.Email);
        _userRepository.Insert(user);
        await _userRepository.SaveChangesAsync(cancellationToken);
        
        return user.Id;
    }
}