using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace Application.Common.Interfaces;

public interface IApplicationDbContext
{
    DbSet<TodoItem> TodoItems { get; }

    DbSet<Group> Groups { get; }

    DbSet<Message> Messages { get; }

    DbSet<DomainUserGroup> DomainUserGroups { get; }

    DbSet<InvitationToGroup> Invitations { get; }

    DbSet<RefreshToken> RefreshTokens { get; }

    DbSet<Event> Events { get; }

    DbSet<MobileCode> MobileCodes { get; }

    public DatabaseFacade DatabaseFacade { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken);
}