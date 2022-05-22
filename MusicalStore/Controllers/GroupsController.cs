using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MusicalStore.Models.Dtos.Groups;
using MusicalStore.Models.Entities;

namespace MusicalStore.Controllers;

[Authorize(PolicyNames.Admin)]
public class GroupsController : Controller
{
    private readonly MusicalStoreContext _context;

    public GroupsController(MusicalStoreContext context)
    {
        _context = context;
    }

    // GET: Groups
    public async Task<IActionResult> Index()
    {
        var groups = await _context.Groups
            .Select(x => new GroupDto
            {
                Id = x.Id,
                GroupName = x.GroupName,
                Types = x.Types.Select(type => type.TypeName)
            })
            .ToListAsync();
            
        return View(groups);
    }

    // GET: Groups/Details/5
    public async Task<IActionResult> Details(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var group = await _context.Groups
            .Where(x => x.Id == id.Value)
            .Select(x => new GroupDto
            {
                Id = x.Id,
                GroupName = x.GroupName,
                Types = x.Types.Select(type => type.TypeName)
            })
            .FirstOrDefaultAsync();
            
        if (group == null)
        {
            return NotFound();
        }

        return View(group);
    }

    // GET: Groups/Create
    public IActionResult Create()
    {
        return View();
    }

    // POST: Groups/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(GroupWriteDto dto)
    {
        if (!ModelState.IsValid)
            return View(dto);

        if (await _context.Groups.AnyAsync(x => x.GroupName.ToLower() == dto.GroupName.ToLower()))
        {
            ModelState.AddModelError("groupName", "Group with the same name already exists");
            return View(dto);
        }
            
        var group = new Group
        {
            GroupName = dto.GroupName,
        };

        group.Types = dto.Types.Select(x => new InstrumentType
        {
            TypeName = x,
            Group = group
        }).ToList();
            
        _context.Add(group);
        await _context.SaveChangesAsync();
            
        return RedirectToAction(nameof(Index));
    }

    // GET: Groups/Edit/5
    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var group = await _context.Groups
            .Where(x => x.Id == id.Value)
            .Select(x => new GroupWriteDto
            {
                GroupName = x.GroupName,
                Types = x.Types.Select(type => type.TypeName)
            })
            .FirstOrDefaultAsync();
            
        if (group == null)
        {
            return NotFound();
        }
        return View(group);
    }

    // POST: Groups/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, GroupWriteDto dto)
    {
        if (!ModelState.IsValid)
            return View(dto);

        var group = await _context.Groups
            .Include(x => x.Types)
            .FirstOrDefaultAsync(x => x.Id == id);

        if (group is null)
        {
            return NotFound();
        }

        group.GroupName = dto.GroupName;

        var newTypes = dto.Types
            .Except(group.Types.Select(x => x.TypeName))
            .Select(x => new InstrumentType
            {
                Group = group,
                TypeName = x
            });

        group.Types = group.Types
            .IntersectBy(dto.Types, type => type.TypeName)
            .Concat(newTypes)
            .ToList();

        await _context.SaveChangesAsync();
            
        return RedirectToAction(nameof(Index));
    }

    // GET: Groups/Delete/5
    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var group = await _context.Groups
            .Where(x => x.Id == id.Value)
            .Select(x => new GroupDto
            {
                Id = x.Id,
                GroupName = x.GroupName,
                Types = x.Types.Select(type => type.TypeName)
            })
            .FirstOrDefaultAsync();
            
        if (group == null)
        {
            return NotFound();
        }

        return View(group);
    }

    // POST: Groups/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var group = await _context.Groups
            .Include(x => x.Types)
            .FirstOrDefaultAsync(x => x.Id == id);

        if (group is not null)
        {
            _context.RemoveRange(group.Types);
            _context.Remove(group);
            await _context.SaveChangesAsync();   
        }

        return RedirectToAction(nameof(Index));
    }
}