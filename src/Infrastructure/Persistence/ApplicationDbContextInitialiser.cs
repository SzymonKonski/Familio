using Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Persistence;

public class ApplicationDbContextInitialiser
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<ApplicationDbContextInitialiser> _logger;
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly UserManager<DomainUser> _userManager;

    public ApplicationDbContextInitialiser(ILogger<ApplicationDbContextInitialiser> logger,
        ApplicationDbContext context, UserManager<DomainUser> userManager, RoleManager<IdentityRole> roleManager)
    {
        _logger = logger;
        _context = context;
        _userManager = userManager;
        _roleManager = roleManager;
    }

    public async Task InitialiseAsync()
    {
        try
        {
            if (_context.Database.IsSqlServer()) await _context.Database.MigrateAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while initialising the database.");
            throw;
        }
    }

    public async Task SeedAsync()
    {
        try
        {
            await TrySeedAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while seeding the database.");
            throw;
        }
    }

    public async Task TrySeedAsync()
    {
        var user1 = new DomainUser
        {
            Email = "pkwola@gmail.com",
            Firstname = "szymon",
            Surname = "konski",
            UserName = "pkwola@gmail.com",
            DomainUsername = "Szymon Konski"
        };

        var user2 = new DomainUser
        {
            Email = "szymon.konski@outlook.com",
            Firstname = "emil",
            Surname = "konski",
            UserName = "szymon.konski@outlook.com",
            DomainUsername = "Emil Konski"
        };

        var result1 = await _userManager.CreateAsync(user1, "Test1234!");
        var result2 = await _userManager.CreateAsync(user2, "Test1234!");

        await _context.SaveChangesAsync();

        var code1 = await _userManager.GenerateEmailConfirmationTokenAsync(user1);
        var code2 = await _userManager.GenerateEmailConfirmationTokenAsync(user2);

        await _userManager.ConfirmEmailAsync(user1, code1);
        await _userManager.ConfirmEmailAsync(user2, code2);

        var groupId1 = Guid.NewGuid().ToString();
        _context.Groups.Add(new Group
        {
            Id = groupId1,
            Name = "Group1"
        });

        var groupId2 = Guid.NewGuid().ToString();
        _context.Groups.Add(new Group
        {
            Id = groupId2,
            Name = "Group2"
        });

        await _context.SaveChangesAsync();

        var group1 = await _context.Groups.FirstOrDefaultAsync(x => x.Id == groupId1);
        var group2 = await _context.Groups.FirstOrDefaultAsync(x => x.Id == groupId2);
        var userr1 = await _userManager.Users.FirstOrDefaultAsync(u => u.Email == "pkwola@gmail.com");
        var userr2 = await _userManager.Users.FirstOrDefaultAsync(u => u.Email == "szymon.konski@outlook.com");

        _context.DomainUserGroups.Add(new DomainUserGroup
        {
            DomainUser = userr1,
            DomainUserId = userr1.Id,
            Group = group1,
            GroupId = group1.Id,
            Role = "Parent",
            UserName = "pkwola"
        });

        _context.DomainUserGroups.Add(new DomainUserGroup
        {
            DomainUser = userr1,
            DomainUserId = userr1.Id,
            Group = group2,
            GroupId = group2.Id,
            Role = "Parent",
            UserName = "szymon"
        });

        _context.DomainUserGroups.Add(new DomainUserGroup
        {
            DomainUser = userr2,
            DomainUserId = userr2.Id,
            Group = group2,
            GroupId = group2.Id,
            Role = "Child",
            UserName = "pkwola"
        });

        await _context.SaveChangesAsync();
    }
}