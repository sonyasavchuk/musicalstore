using Microsoft.AspNetCore.Identity;

namespace MusicalStore.Models.Entities.Identity;

public class ApplicationRole : IdentityRole<string>
{
    public ApplicationRole(string name) : base(name) {}

    public ICollection<ApplicationUser> Users { get; set; } = new HashSet<ApplicationUser>();
}
