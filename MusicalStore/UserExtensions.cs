using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using MusicalStore.Models.Entities.Identity;

namespace MusicalStore;

public static class UserExtensions
{
    public static bool IsAdmin(this SignInManager<ApplicationUser> manager, ClaimsPrincipal user)
    {
        return manager.IsSignedIn(user) && user.IsInRole(RoleNames.Admin);
    }
}
