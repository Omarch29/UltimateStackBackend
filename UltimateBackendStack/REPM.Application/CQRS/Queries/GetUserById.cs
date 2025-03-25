using Ardalis.GuardClauses;
using AutoMapper;
using MediatR;
using REPM.Application.DTOs;
using REPM.Domain.Entities;
using REPM.Infrastructure.Interfaces;

namespace REPM.Application.CQRS.Queries;

public record GetUserById(Guid Id) : IRequest<UserDto>
{
}

public class GetUserByIdHandler : IRequestHandler<GetUserById, UserDto>
{
    
    private readonly IMapper _mapper;
    private readonly IRepository<User> _userRepository;
    
    public GetUserByIdHandler(IMapper mapper, IRepository<User> userRepository)
    {
        _mapper = mapper;
        _userRepository = userRepository;
    }
    
    public async Task<UserDto> Handle(GetUserById request, CancellationToken cancellationToken)
    {
        Guard.Against.Null(request, nameof(request));
        Guard.Against.Null(request.Id, nameof(request.Id));
        
        var user = await _userRepository.GetByIdAsync(request.Id);
        if (user != null)
        {
            return _mapper.Map<UserDto>(user);
        }
        
        throw new NotFoundException(nameof(User), request.Id.ToString());
    }
}