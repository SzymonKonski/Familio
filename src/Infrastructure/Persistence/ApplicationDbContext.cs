using System.Reflection;
using Application.Common.Interfaces;
using Domain.Entities;
using Infrastructure.Persistence.Interceptors;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace Infrastructure.Persistence;

public class ApplicationDbContext : IdentityUserContext<DomainUser, string>, IApplicationDbContext
{
    private readonly AuditableEntitySaveChangesInterceptor _auditableEntitySaveChangesInterceptor;

    public ApplicationDbContext(
        DbContextOptions<ApplicationDbContext> options,
        AuditableEntitySaveChangesInterceptor auditableEntitySaveChangesInterceptor)
        : base(options)
    {
        _auditableEntitySaveChangesInterceptor = auditableEntitySaveChangesInterceptor;
    }

    public DbSet<MobileCode> MobileCodes => Set<MobileCode>();

    public DbSet<Event> Events => Set<Event>();

    public DatabaseFacade DatabaseFacade => Database;

    public DbSet<TodoItem> TodoItems => Set<TodoItem>();

    public DbSet<Group> Groups => Set<Group>();

    public DbSet<DomainUserGroup> DomainUserGroups => Set<DomainUserGroup>();

    public DbSet<Message> Messages => Set<Message>();

    public DbSet<InvitationToGroup> Invitations => Set<InvitationToGroup>();

    public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        builder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        base.OnModelCreating(builder);
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.AddInterceptors(_auditableEntitySaveChangesInterceptor);
    }
}