using Application.Common.Interfaces;
using Domain.Entities;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace Infrastructure.Identity;

public class InvitationDataProtector : IInvitationDataProtector
{
    private readonly IApplicationDbContext _context;

    public InvitationDataProtector(IDataProtectionProvider dataProtectionProvider,
        IOptions<DataProtectionTokenProviderOptions> options,
        IApplicationDbContext context)
    {
        if (dataProtectionProvider == null) throw new ArgumentNullException(nameof(dataProtectionProvider));
        _context = context;
        Options = options?.Value ?? new DataProtectionTokenProviderOptions();
        Protector = dataProtectionProvider.CreateProtector(Name ?? "DataProtectorTokenProvider");
    }

    protected IDataProtector Protector { get; }
    public string Name => Options.Name;
    protected DataProtectionTokenProviderOptions Options { get; }


    public async Task<InvitationToGroup> GetDataFromToken(string token)
    {
        var unprotectedData = Protector.Unprotect(Convert.FromBase64String(token));
        var ms = new MemoryStream(unprotectedData);

        using var reader = ms.CreateReader();
        var creationTime = reader.ReadDateTimeOffset();
        var userId = reader.ReadString();
        var groupId = reader.ReadString();
        var role = reader.ReadString();
        var stamp = reader.ReadString();

        var invitation = await _context.Invitations.FirstOrDefaultAsync(x =>
            x.DomainUserId == userId && x.GroupId == groupId && x.Role == role);

        return invitation;
    }
}