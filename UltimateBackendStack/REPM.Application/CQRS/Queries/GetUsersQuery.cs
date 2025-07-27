using AutoMapper;
using MediatR;
using REPM.Application.DTOs;
using REPM.Domain.Entities;
using REPM.Infrastructure.Interfaces;

namespace REPM.Application.CQRS.Queries;

public record GetUsersQuery : IRequest<IEnumerable<UserDto>>
{
}

public class GetUsersQueryHandler : IRequestHandler<GetUsersQuery, IEnumerable<UserDto>>
{
    private readonly IMapper _mapper;
    private readonly IRepository<User> _userRepository;
    
    public GetUsersQueryHandler(IMapper mapper, IRepository<User> userRepository)
    {
        _mapper = mapper;
        _userRepository = userRepository;
    }
    
    public Task<IEnumerable<UserDto>> Handle(GetUsersQuery request, CancellationToken cancellationToken)
    {
        var users = _userRepository.QueryToRead.ToList();
        return Task.FromResult(_mapper.Map<IEnumerable<UserDto>>(users));
    }
}
