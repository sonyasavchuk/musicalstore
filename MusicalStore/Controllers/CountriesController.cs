using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MusicalStore.Models.Dtos.Countries;
using MusicalStore.Models.Entities;

namespace MusicalStore.Controllers;

[Authorize(PolicyNames.Admin)]
public class CountriesController : Controller
{
    private readonly MusicalStoreContext _context;

    public CountriesController(MusicalStoreContext context)
    {
        _context = context;
    }

    // GET: Countries
    public async Task<IActionResult> Index()
    {
        var countries = await _context.Countries
            .Select(x => new CountryDto
            {
                Id = x.Id,
                CountryName = x.CountryName,
                InstrumentsCount = x.Instruments.Count,
                ManufacturersCount = x.Manufacturers.Count
            })
            .ToListAsync();
            
        return View(countries);
    }

    // GET: Countries/Details/5
    public async Task<IActionResult> Details(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var country = await _context.Countries
            .Where(x => x.Id == id.Value)
            .Select(x => new CountryDto
            {
                Id = x.Id,
                CountryName = x.CountryName,
                InstrumentsCount = x.Instruments.Count,
                ManufacturersCount = x.Manufacturers.Count
            })
            .FirstOrDefaultAsync();
            
        if (country == null)
        {
            return NotFound();
        }

        return View(country);
    }

    // GET: Countries/Create
    public IActionResult Create()
    {
        return View();
    }

    // POST: Countries/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CountryWriteDto dto)
    {
        if (!ModelState.IsValid)
            return View(dto);

        if (await _context.Countries.AnyAsync(x => x.CountryName.ToLower() == dto.CountryName.ToLower()))
        {
            ModelState.AddModelError("countryName", "Country with the same name already exists");
            return View(dto);
        }
            
        var country = new Country
        {
            CountryName = dto.CountryName
        };
            
        _context.Add(country);
        await _context.SaveChangesAsync();
            
        return RedirectToAction(nameof(Index));
    }

    // GET: Countries/Edit/5
    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var country = await _context.Countries
            .Where(x => x.Id == id.Value)
            .Select(x => new CountryWriteDto
            {
                CountryName = x.CountryName
            })
            .FirstOrDefaultAsync();
            
        if (country == null)
        {
            return NotFound();
        }
            
        return View(country);
    }

    // POST: Countries/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, CountryWriteDto dto)
    {
        if (!ModelState.IsValid)
            return View(dto);

        var country = await _context.Countries.FirstOrDefaultAsync(x => x.Id == id);

        if (country is null)
        {
            return NotFound();
        }

        country.CountryName = dto.CountryName;

        await _context.SaveChangesAsync();
            
        return RedirectToAction(nameof(Index));
    }

    // GET: Countries/Delete/5
    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var country = await _context.Countries
            .Where(x => x.Id == id.Value)
            .Select(x => new CountryDto
            {
                Id = x.Id,
                CountryName = x.CountryName,
                InstrumentsCount = x.Instruments.Count,
                ManufacturersCount = x.Manufacturers.Count
            })
            .FirstOrDefaultAsync();
            
        if (country == null)
        {
            return NotFound();
        }

        return View(country);
    }

    // POST: Countries/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var country = await _context.Countries
            .Include(x => x.Manufacturers)
            .FirstOrDefaultAsync(x => x.Id == id);

        if (country is not null)
        {
            _context.RemoveRange(country.Manufacturers);
            _context.Countries.Remove(country);
            await _context.SaveChangesAsync();
        }

        return RedirectToAction(nameof(Index));
    }
}