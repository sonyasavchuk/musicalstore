using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MusicalStore.Models.Dtos.Manufacturers;
using MusicalStore.Models.Entities;

namespace MusicalStore.Controllers;

[Authorize]
public class ManufacturersController : Controller
{
    private readonly MusicalStoreContext _context;

    public ManufacturersController(MusicalStoreContext context)
    {
        _context = context;
    }

    // GET: Manufacturers
    public async Task<IActionResult> Index()
    {
        var manufacturers = await _context.Manufacturers
            .Select(x => new ManufacturerDto
            {
                Id = x.Id,
                CountryName = x.Country.CountryName,
                ManufacturerName = x.ManufacturerName,
                InstrumentsCount = x.Instruments.Count
            })
            .ToListAsync();
            
        return View(manufacturers);
    }

    // GET: Manufacturers/Details/5
    public async Task<IActionResult> Details(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var manufacturer = await _context.Manufacturers
            .Where(x => x.Id == id.Value)
            .Select(x => new ManufacturerDto
            {
                Id = x.Id,
                CountryName = x.Country.CountryName,
                ManufacturerName = x.ManufacturerName,
                InstrumentsCount = x.Instruments.Count
            })
            .FirstOrDefaultAsync();
            
        if (manufacturer == null)
        {
            return NotFound();
        }

        return View(manufacturer);
    }

    // GET: Manufacturers/Create
    public async Task<IActionResult> Create()
    {
        await LoadViewData();
        return View();
    }

    // POST: Manufacturers/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(ManufacturerWriteDto dto)
    {
        if (!ModelState.IsValid)
        {
            await LoadViewData();
            return View(dto);
        }

        if (await _context.Manufacturers.AnyAsync(x => x.ManufacturerName.ToLower() == dto.ManufacturerName.ToLower()))
        {
            ModelState.AddModelError("manufacturerName", "Manufacturer with the same name already exists");
            await LoadViewData();
            return View(dto);
        }
            
        var manufacturer = new Manufacturer
        {
            ManufacturerName = dto.ManufacturerName,
            Country = await _context.Countries.FirstOrDefaultAsync(x => dto.CountryId == x.Id)
        };
            
        _context.Add(manufacturer);
        await _context.SaveChangesAsync();
            
        return RedirectToAction(nameof(Index));
    }

    // GET: Manufacturers/Edit/5
    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var manufacturer = await _context.Manufacturers
            .Where(x => x.Id == id.Value)
            .Select(x => new ManufacturerWriteDto
            {
                ManufacturerName = x.ManufacturerName,
                CountryId = x.CountryId
            })
            .FirstOrDefaultAsync();
            
        if (manufacturer == null)
        {
            return NotFound();
        }

        await LoadViewData();
        return View(manufacturer);
    }

    // POST: Manufacturers/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, ManufacturerWriteDto dto)
    {
        if (!ModelState.IsValid)
        {
            await LoadViewData();
            return View(dto);
        }

        var manufacturer = await _context.Manufacturers
            .Include(x => x.Country)
            .FirstOrDefaultAsync(x => x.Id == id);

        if (manufacturer is null)
        {
            return NotFound();
        }

        manufacturer.ManufacturerName = dto.ManufacturerName;

        manufacturer.Country = await _context.Countries
            .FirstOrDefaultAsync(x => x.Id == dto.CountryId);
            
        await _context.SaveChangesAsync();
            
        return RedirectToAction(nameof(Index));
    }

    // GET: Manufacturers/Delete/5
    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var manufacturer = await _context.Manufacturers
            .Where(x => x.Id == id.Value)
            .Select(x => new ManufacturerDto
            {
                Id = x.Id,
                CountryName = x.Country.CountryName,
                ManufacturerName = x.ManufacturerName,
                InstrumentsCount = x.Instruments.Count
            })
            .FirstOrDefaultAsync();
            
        if (manufacturer == null)
        {
            return NotFound();
        }

        return View(manufacturer);
    }

    // POST: Manufacturers/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var manufacturer = await _context.Manufacturers.FindAsync(id);
            
        _context.Manufacturers.Remove(manufacturer);
        await _context.SaveChangesAsync();
            
        return RedirectToAction(nameof(Index));
    }

    private async Task LoadViewData()
    {
        var countries = await _context.Countries
            .Select(x => new
            {
                Id = x.Id,
                CountryName = x.CountryName
            })
            .ToListAsync();

        ViewData["Countries"] = new SelectList(countries, "Id", "CountryName");
    }
}