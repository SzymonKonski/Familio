using System.Text;
using Application.Common.Interfaces;
using Domain.Entities;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Infrastructure.Identity;

public class CustomTokenProvider<TUser> : IUserTwoFactorTokenProvider<TUser> where TUser : DomainUser
{
    private readonly IApplicationDbContext _context;

    public CustomTokenProvider(IDataProtectionProvider dataProtectionProvider,
        IOptions<DataProtectionTokenProviderOptions> options,
        ILogger<DataProtectorTokenProvider<TUser>> logger,
        IApplicationDbContext context)
    {
        if (dataProtectionProvider == null) throw new ArgumentNullException(nameof(dataProtectionProvider));

        Options = options?.Value ?? new DataProtectionTokenProviderOptions();

        // Use the Name as the purpose which should usually be distinct from others
        Protector = dataProtectionProvider.CreateProtector(Name ?? "DataProtectorTokenProvider");
        Logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _context = context;
    }

    protected IDataProtector Protector { get; }
    protected DataProtectionTokenProviderOptions Options { get; }
    public string Name => Options.Name;
    public ILogger<DataProtectorTokenProvider<TUser>> Logger { get; }

    public async Task<bool> ValidateAsync(string purpouse, string token, UserManager<TUser> manager, TUser user)
    {
        try
        {
            var unprotectedData = Protector.Unprotect(Convert.FromBase64String(token));
            var ms = new MemoryStream(unprotectedData);
            using var reader = ms.CreateReader();
            var creationTime = reader.ReadDateTimeOffset();
            var expirationTime = creationTime + Options.TokenLifespan;
            if (expirationTime < DateTimeOffset.UtcNow)
            {
                Logger.InvalidExpirationTime();
                return false;
            }

            var userId = reader.ReadString();
            var actualUserId = await manager.GetUserIdAsync(user);
            if (userId != actualUserId)
            {
                Logger.UserIdsNotEquals();
                return false;
            }

            var groupId = reader.ReadString();
            var role = reader.ReadString();

            var stamp = reader.ReadString();
            if (reader.PeekChar() != -1)
            {
                Logger.UnexpectedEndOfInput();
                return false;
            }

            var invitation = await _context.Invitations.FirstOrDefaultAsync(x =>
                x.DomainUserId == userId && x.GroupId == groupId && x.Role == role);

            if (invitation == null)
            {
                Logger.InvitationNotEquals();
                return false;
            }

            if (manager.SupportsUserSecurityStamp)
            {
                var isEqualsSecurityStamp = stamp == await manager.GetSecurityStampAsync(user);
                if (!isEqualsSecurityStamp) Logger.SecurityStampNotEquals();

                return isEqualsSecurityStamp;
            }


            var stampIsEmpty = stamp == "";
            if (!stampIsEmpty) Logger.SecurityStampIsNotEmpty();

            return stampIsEmpty;
        }
        // ReSharper disable once EmptyGeneralCatchClause
        catch
        {
            // Do not leak exception
            Logger.UnhandledException();
        }

        return false;
    }

    public Task<bool> CanGenerateTwoFactorTokenAsync(UserManager<TUser> manager, TUser user)
    {
        return Task.FromResult(manager != null && user != null);
    }

    public async Task<string> GenerateAsync(string groupId, UserManager<TUser> manager, TUser user)
    {
        if (user == null) throw new ArgumentNullException(nameof(user));
        var userId = await manager.GetUserIdAsync(user);

        var invitation =
            await _context.Invitations.FirstOrDefaultAsync(x => x.DomainUserId == userId && x.GroupId == groupId);

        var ms = new MemoryStream();
        await using (var writer = ms.CreateWriter())
        {
            writer.Write(DateTimeOffset.UtcNow);
            writer.Write(userId);
            writer.Write(invitation.GroupId);
            writer.Write(invitation.Role);
            string stamp = null;
            if (manager.SupportsUserSecurityStamp) stamp = await manager.GetSecurityStampAsync(user);
            writer.Write(stamp ?? "");
        }

        var protectedBytes = Protector.Protect(ms.ToArray());
        return Convert.ToBase64String(protectedBytes);
    }
}

internal static class StreamExtensions
{
    internal static readonly Encoding DefaultEncoding = new UTF8Encoding(false, true);

    public static BinaryReader CreateReader(this Stream stream)
    {
        return new BinaryReader(stream, DefaultEncoding, true);
    }

    public static BinaryWriter CreateWriter(this Stream stream)
    {
        return new BinaryWriter(stream, DefaultEncoding, true);
    }

    public static DateTimeOffset ReadDateTimeOffset(this BinaryReader reader)
    {
        return new DateTimeOffset(reader.ReadInt64(), TimeSpan.Zero);
    }

    public static void Write(this BinaryWriter writer, DateTimeOffset value)
    {
        writer.Write(value.UtcTicks);
    }
}

internal static class LoggingExtensions
{
    private static readonly Action<ILogger, Exception> _invalidExpirationTime;
    private static readonly Action<ILogger, Exception> _userIdsNotEquals;
    private static readonly Action<ILogger, Exception> _invitationNotEquals;
    private static readonly Action<ILogger, Exception> _unexpectedEndOfInput;
    private static readonly Action<ILogger, Exception> _securityStampNotEquals;
    private static readonly Action<ILogger, Exception> _securityStampIsNotEmpty;
    private static readonly Action<ILogger, Exception> _unhandledException;

    static LoggingExtensions()
    {
        _invalidExpirationTime = LoggerMessage.Define(
            eventId: new EventId(0, "InvalidExpirationTime"),
            logLevel: LogLevel.Debug,
            formatString: "ValidateAsync failed: the expiration time is invalid.");

        _userIdsNotEquals = LoggerMessage.Define(
            eventId: new EventId(1, "UserIdsNotEquals"),
            logLevel: LogLevel.Debug,
            formatString: "ValidateAsync failed: did not find expected UserId.");

        _invitationNotEquals = LoggerMessage.Define(
            eventId: new EventId(2, "InvitationNotEquals"),
            logLevel: LogLevel.Debug,
            formatString: "ValidateAsync failed: did not find Invitation in database.");

        _unexpectedEndOfInput = LoggerMessage.Define(
            eventId: new EventId(3, "UnexpectedEndOfInput"),
            logLevel: LogLevel.Debug,
            formatString: "ValidateAsync failed: unexpected end of input.");

        _securityStampNotEquals = LoggerMessage.Define(
            eventId: new EventId(4, "SecurityStampNotEquals"),
            logLevel: LogLevel.Debug,
            formatString: "ValidateAsync failed: did not find expected security stamp.");

        _securityStampIsNotEmpty = LoggerMessage.Define(
            eventId: new EventId(5, "SecurityStampIsNotEmpty"),
            logLevel: LogLevel.Debug,
            formatString: "ValidateAsync failed: the expected stamp is not empty.");

        _unhandledException = LoggerMessage.Define(
            eventId: new EventId(6, "UnhandledException"),
            logLevel: LogLevel.Debug,
            formatString: "ValidateAsync failed: unhandled exception was thrown.");
    }

    public static void InvalidExpirationTime(this ILogger logger)
    {
        _invalidExpirationTime(logger, null);
    }

    public static void UserIdsNotEquals(this ILogger logger)
    {
        _userIdsNotEquals(logger, null);
    }

    public static void InvitationNotEquals(this ILogger logger)
    {
        _invitationNotEquals(logger, null);
    }

    public static void UnexpectedEndOfInput(this ILogger logger)
    {
        _unexpectedEndOfInput(logger, null);
    }

    public static void SecurityStampNotEquals(this ILogger logger)
    {
        _securityStampNotEquals(logger, null);
    }

    public static void SecurityStampIsNotEmpty(this ILogger logger)
    {
        _securityStampIsNotEmpty(logger, null);
    }

    public static void UnhandledException(this ILogger logger)
    {
        _unhandledException(logger, null);
    }
}