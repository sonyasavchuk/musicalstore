using System.Linq.Expressions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MusicalStore.Extensions;
using MusicalStore.Models.Dtos.Instruments;
using MusicalStore.Models.Entities;
using MusicalStore.Models.ExternalData;
using MusicalStore.Services;
using MusicalStore.Services.ExternalData;

namespace MusicalStore.Controllers;

[Authorize]
public class InstrumentsController : Controller
{
    private readonly MaterialsService _materialsService;
    private readonly MusicalStoreContext _context;
    private readonly ExcelDataService _excelDataService;
    private readonly DocxDataService _docxDataService;
    private readonly DataTransformationService _transformationService;

    private Expression<Func<Instrument, InstrumentDto>> _instrumentMapper = x => new InstrumentDto
    {
        Id = x.Id,
        Name = x.Name,
        Price = x.Price,
        Materials = x.Materials.Select(material => material.MaterialName).ToList(),
        InstrumentType = x.InstrumentType.TypeName,
        ManufacturerName = x.Manufacturer.ManufacturerName,
        ManufactoringCountryName = x.ManufactoringCountry.CountryName
    };
        
    public InstrumentsController(MusicalStoreContext context, MaterialsService materialsService, ExcelDataService excelDataService, DocxDataService docxDataService, DataTransformationService transformationService)
    {
        _context = context;
        _materialsService = materialsService;
        _excelDataService = excelDataService;
        _docxDataService = docxDataService;
        _transformationService = transformationService;
    }

    // GET: Instruments
    public async Task<IActionResult> Index()
    {
        var instruments = await _context.Instruments
            .Select(_instrumentMapper)
            .ToListAsync();

        ViewBag.SelectableInstruments = new SelectList(instruments, "Id", "Name");
            
        return View(instruments);
    }

    // GET: Instruments/Details/5
    public async Task<IActionResult> Details(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var instrument = await _context.Instruments
            .Where(x => x.Id == id.Value)
            .Select(_instrumentMapper)
            .FirstOrDefaultAsync();
            
        if (instrument == null)
        {
            return NotFound();
        }

        return View(instrument);
    }

    // GET: Instruments/Create
    public async Task<IActionResult> Create()
    {
        await LoadViewData();
        return View();
    }

    // POST: Instruments/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(InstrumentWriteDto dto)
    {
        if (!ModelState.IsValid)
        {
            await LoadViewData();
            return View(dto);
        }

        var materials = (await _materialsService.GetOrAddMaterialsByNames(dto.Materials)).ToList();
            
        var instrument = new Instrument
        {
            Name = dto.Name,
            Price = dto.Price,
            InstrumentTypeId = dto.InstrumentTypeId,
            ManufacturerId = dto.ManufacturerId,
            ManufactoringCountryId = dto.ManufactoringCountryId,
            Materials = materials
        };

        _context.Add(instrument);
            
        await _context.SaveChangesAsync();
            
        return RedirectToAction(nameof(Index));
    }

    // GET: Instruments/Edit/5
    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var instrument = await _context.Instruments
            .Where(x => x.Id == id.Value)
            .Select(x => new InstrumentWriteDto
            {
                Name = x.Name,
                Price = x.Price,
                Materials = x.Materials.Select(mat => mat.MaterialName).ToList(),
                ManufacturerId = x.ManufacturerId,
                InstrumentTypeId = x.InstrumentTypeId,
                ManufactoringCountryId = x.ManufactoringCountryId
            })
            .FirstOrDefaultAsync();
            
        if (instrument == null)
        {
            return NotFound();
        }

        await LoadViewData();
        return View(instrument);
    }

    // POST: Instruments/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, InstrumentWriteDto dto)
    {
        if (!ModelState.IsValid)
        {
            await LoadViewData();
            return View(dto);
        }

        var instrument = await _context.Instruments
            .Include(x => x.Materials)
            .FirstOrDefaultAsync(x => x.Id == id);

        if (instrument is null)
        {
            return NotFound();
        }
            
        instrument.Name = dto.Name;
        instrument.Price = dto.Price;
        instrument.InstrumentTypeId = dto.InstrumentTypeId;
        instrument.ManufacturerId = dto.ManufacturerId;
        instrument.ManufactoringCountryId = dto.ManufactoringCountryId;
        instrument.Materials = (await _materialsService.GetOrAddMaterialsByNames(dto.Materials)).ToList(); 

        await _context.SaveChangesAsync();
            
        return RedirectToAction(nameof(Index));
    }

    // GET: Instruments/Delete/5
    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var instrument = await _context.Instruments
            .Where(m => m.Id == id)
            .Select(_instrumentMapper)
            .FirstOrDefaultAsync();
            
        if (instrument == null)
        {
            return NotFound();
        }

        return View(instrument);
    }

    // POST: Instruments/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var instrument = await _context.Instruments
            .Include(x => x.Materials)
            .FirstOrDefaultAsync(x => x.Id == id);

        if (instrument is not null)
        {
            instrument.Materials = new List<Material>();
            _context.Instruments.Remove(instrument);
            await _context.SaveChangesAsync();
        }

        return RedirectToAction(nameof(Index));
    }

    [HttpGet, ActionName("ExportSpreadsheet")]
    public async Task<IActionResult> ExportSpreadsheet(IEnumerable<int> selectedInstrumentIds = null)
    {
        var table = await GetExportTable(selectedInstrumentIds);
            
        using var memoryStream = new MemoryStream();
        _excelDataService.ExportSpreadsheetToStream(memoryStream, table);
        await memoryStream.FlushAsync();
            
        return new FileContentResult(memoryStream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet")
        {
            FileDownloadName = $"InstrumentsExport-{DateTime.Now}.xlsx"
        };
    }
        
    [HttpGet, ActionName("ExportDocument")]
    public async Task<IActionResult> ExportDocument(IEnumerable<int> selectedInstrumentIds = null)
    {
        var table = await GetExportTable(selectedInstrumentIds);

        using var memoryStream = new MemoryStream();
        _docxDataService.ExportDocumentToStream(memoryStream, table);
        await memoryStream.FlushAsync();
            
        return new FileContentResult(memoryStream.ToArray(), "application/vnd.openxmlformats-officedocument.wordprocessingml.document")
        {
            FileDownloadName = $"InstrumentsExport-{DateTime.Now}.docx"
        };
    }

    [HttpPost, ActionName("ImportSpreadsheet")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ImportFromSpreadsheet(IFormFile spreadsheetFile)
    {
        if (!ModelState.IsValid)
        {
            return RedirectToAction(nameof(Index));
        }

        try
        {
            await using var fileStream = spreadsheetFile.OpenReadStream();
            var tableData = _excelDataService.ImportFromSpreadsheetStream(fileStream);
            var instruments = _transformationService.FromTableData<InstrumentExternalDto>(tableData);

            await AddImportedInstruments(instruments);
        }
        catch (Exception)
        {
            ModelState.AddModelError("spreadsheetFile", "Invalid spreadsheet");
        }

        return RedirectToAction(nameof(Index));
    }
        
    [HttpPost, ActionName("ImportDocument")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ImportFromDocument(IFormFile documentFile)
    {
        if (!ModelState.IsValid)
        {
            return RedirectToAction(nameof(Index));
        }

        try
        {
            await using var fileStream = documentFile.OpenReadStream();
            var tableData = _docxDataService.ImportFromDocumentStream(fileStream);
            var instruments = _transformationService.FromTableData<InstrumentExternalDto>(tableData);

            await AddImportedInstruments(instruments);
        }
        catch (Exception)
        {
            ModelState.AddModelError("documentFile", "Invalid spreadsheet");
        }

        return RedirectToAction(nameof(Index));
    }

    private async Task AddImportedInstruments(IEnumerable<InstrumentExternalDto> instruments)
    {
        await using var transaction = await _context.Database.BeginTransactionAsync();

        foreach (var dto in instruments)
        {
            var instrument = new Instrument
            {
                Name = dto.Name,
                Price = dto.Price,
                InstrumentType = await _context.Types.FirstAsync(x => x.TypeName == dto.InstrumentTypeName),
                Manufacturer = await _context.Manufacturers.FirstAsync(x => x.ManufacturerName == dto.ManufacturerName),
                ManufactoringCountry = await _context.Countries.FirstAsync(x => x.CountryName == dto.ManufactoringCountryName),
            };

            _context.Add(instrument);

            await _context.SaveChangesAsync();
                
            instrument.Materials = (await _materialsService.GetOrAddMaterialsByNames(dto.Materials)).ToList();
                
            await _context.SaveChangesAsync();
        }

        await _context.SaveChangesAsync();
        await transaction.CommitAsync();
    }
        
    private async Task<CommonTable> GetExportTable(IEnumerable<int> instrumentIds = null)
    {
        var instrumentDtos = await _context.Instruments
            .Include(x => x.Manufacturer)
            .Include(x => x.ManufactoringCountry)
            .Include(x => x.Materials)
            .Include(x => x.InstrumentType)
            .WhereIf(x => instrumentIds.Contains(x.Id), instrumentIds is not null && instrumentIds.Any())
            .Select(x => new InstrumentExternalDto
            {
                Name = x.Name,
                Price = x.Price,
                ManufacturerName = x.Manufacturer.ManufacturerName,
                InstrumentTypeName = x.InstrumentType.TypeName,
                ManufactoringCountryName = x.ManufactoringCountry.CountryName,
                Materials = x.Materials.Select(x => x.MaterialName)
            })
            .ToListAsync();
            
        return _transformationService.ToTableData(instrumentDtos);
    }
        
    private async Task LoadViewData()
    {
        var countries = await _context.Countries
            .Select(x => new
            {
                Id = x.Id,
                Name = x.CountryName
            })
            .ToListAsync();
        ViewBag.Countries = new SelectList(countries, "Id", "Name");
            
        var manufacturers = await _context.Manufacturers
            .Select(x => new
            {
                Id = x.Id,
                Name = x.ManufacturerName
            })
            .ToListAsync();
        ViewBag.Manufacturers = new SelectList(manufacturers, "Id", "Name");
            
        var types = await _context.Types
            .Select(x => new
            {
                Id = x.Id,
                Name = x.TypeName
            })
            .ToListAsync();
        ViewBag.Types = new SelectList(types, "Id", "Name");
            
        var materials = await _context.Materials
            .Select(x => x.MaterialName)
            .ToListAsync();

        ViewBag.Materials = new SelectList(materials);
    }
}