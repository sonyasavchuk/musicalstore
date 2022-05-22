using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using MusicalStore.Models.Entities.Identity;

namespace MusicalStore;

public static class IdentitySeedExtensions
{
    public static async Task EnsureIdentityInitialized(this WebApplication app)
    {
        using var scope = app.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<AuthDbContext>();
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();

        await context.Database.MigrateAsync();
        
        if (!await context.Users.AnyAsync(x => x.Id == AdminConsts.Id))
        {
            var user = new ApplicationUser
            {
                Id = AdminConsts.Id,
                Email = AdminConsts.Email,
                UserName = AdminConsts.UserName,
                EmailConfirmed = true
            };
            await userManager.CreateAsync(user, AdminConsts.DefaultPassword);
        }

        if (!await context.Roles.AnyAsync(x => x.Name == RoleNames.Admin))
        {
            context.Roles.Add(new(RoleNames.Admin)
            {
                Id = Guid.NewGuid().ToString(),
                NormalizedName = RoleNames.Admin.ToUpperInvariant()
            });
            await context.SaveChangesAsync();
        }


        var adminRole = await context.Roles.FirstAsync(x => x.Name == RoleNames.Admin);
        if (!await context.UserRoles.AnyAsync(x => x.RoleId == adminRole.Id && x.UserId == AdminConsts.Id))
        {
            context.UserRoles.Add(new()
            {
                UserId = AdminConsts.Id,
                RoleId = adminRole.Id
            });
            await context.SaveChangesAsync();
        }
        
        
    }
}
