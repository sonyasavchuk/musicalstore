using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MusicalStore.Models.Dtos.Users;
using MusicalStore.Models.Entities.Identity;

namespace MusicalStore.Controllers;

[Authorize(PolicyNames.Admin)]
public class UsersController : Controller
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly AuthDbContext _context;

    public UsersController(AuthDbContext context, UserManager<ApplicationUser> userManager)
    {
        _userManager = userManager;
        _context = context;
    }

    // GET: Users
    public async Task<IActionResult> Index()
    {
        var users = await _context.Users
            .Select(x => new UserForAdminDto(x.Id, x.UserName, x.Email, x.Roles.Select(role => role.Name).ToArray()))
            .ToListAsync();

        return View(users);
    }

    // GET: Users/Create
    public IActionResult Create()
    {
        return View();
    }

    // POST: Users/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(UserWriteDto dto)
    {
        if (!ModelState.IsValid)
            return View(dto);

        var user = new ApplicationUser
        {
            Id = Guid.NewGuid().ToString(),
            Email = dto.Email,
            UserName = dto.UserName
        };
        await _userManager.CreateAsync(user, user.UserName);

        if (dto.IsAdmin)
        {
            await _userManager.AddToRoleAsync(user, RoleNames.Admin);
        }

        return RedirectToAction(nameof(Index));
    }

    // GET: Users/Edit/5
    public async Task<IActionResult> Edit(string id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var user = await _context.Users.FindAsync(id);
        if (user == null)
        {
            return NotFound();
        }

        return View(new UserWriteDto(user.UserName));
    }

    // POST: Users/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(string id, UserWriteDto dto)
    {
        if (!ModelState.IsValid)
            return View(dto);

        var identityUser = await _userManager.FindByIdAsync(id);

        if (identityUser is null)
        {
            return NotFound();
        }

        await _userManager.SetUserNameAsync(identityUser, dto.UserName);

        if (dto.IsAdmin)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (!await _userManager.IsInRoleAsync(user, RoleNames.Admin))
            {
                await _userManager.AddToRoleAsync(user, RoleNames.Admin);
            }
        }
        else
        {
            var user = await _userManager.FindByIdAsync(id);
            if (await _userManager.IsInRoleAsync(user, RoleNames.Admin))
            {
                await _userManager.RemoveFromRoleAsync(user, RoleNames.Admin);
            }
        }

        return RedirectToAction(nameof(Index));
    }

    // GET: Users/Delete/5
    public async Task<IActionResult> Delete(string id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var user = await _context.Users
            .Where(x => x.Id == id)
            .Select(x => new UserReadDto(x.Id.ToString(), x.UserName))
            .FirstOrDefaultAsync();

        if (user is null)
        {
            return NotFound();
        }

        return View(user);
    }

    // POST: Users/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(string id)
    {
        if (User.FindFirstValue(ClaimTypes.NameIdentifier) == id)
        {
            return RedirectToAction(nameof(Index));
        }

        var user = await _context.Users.FindAsync(id);

        if (user is null)
            return RedirectToAction(nameof(Index));

        _context.Users.Remove(user);
        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }
}
