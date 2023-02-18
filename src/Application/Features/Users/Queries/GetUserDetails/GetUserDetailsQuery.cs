using Application.Common.Exceptions;
using Application.Common.Interfaces;
using Application.Security;
using AutoMapper;
using Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Users.Queries.GetUserDetails;

[Authorize(Roles = "Parent,Child,Relative")]
public record GetUserDetailsQuery : IRequest<UserDetailsDto>;

public class GetUserDetailsQueryHandler : IRequestHandler<GetUserDetailsQuery, UserDetailsDto>
{
    private readonly ICurrentUserService _currentUserService;
    private readonly IMapper _mapper;
    private readonly UserManager<DomainUser> _userManager;

    public GetUserDetailsQueryHandler(UserManager<DomainUser> userManager, IMapper mapper,
        ICurrentUserService currentUserService)
    {
        _userManager = userManager;
        _mapper = mapper;
        _currentUserService = currentUserService;
    }

    public async Task<UserDetailsDto> Handle(GetUserDetailsQuery request, CancellationToken cancellationToken)
    {
        var user = await _userManager.Users.FirstOrDefaultAsync(u => u.Id == _currentUserService.UserId,
            cancellationToken);
        if (user == null) throw new NotFoundException(nameof(DomainUser), _currentUserService.UserId);

        return _mapper.Map<DomainUser, UserDetailsDto>(user);
    }
}