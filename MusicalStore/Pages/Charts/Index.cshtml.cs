using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace MusicalStore.Pages.Charts;

public class ChartsPageModel : PageModel
{
    private readonly MusicalStoreContext _context;

    public ICollection<MaterialInfo> MaterialInfos { get; private set; }
    
    public ICollection<ManufacturerInfo> ManufacturerInfos { get; private set; }

    public class MaterialInfo
    {
        public string MaterialName { get; set; }
        public int InstrumentsCount { get; set; }
    }

    public class ManufacturerInfo
    {
        public string ManufacturerName { get; set; }
        public decimal? AverageInstrumentPrice { get; set; }
    }
    
    public ChartsPageModel(MusicalStoreContext context)
    {
        _context = context;
    }
    
    public async Task OnGet()
    {
        MaterialInfos = await _context.Materials
            .Select(x => new MaterialInfo
            {
                MaterialName = x.MaterialName,
                InstrumentsCount = x.Instruments.Count
            })
            .ToListAsync();

        ManufacturerInfos = await _context.Manufacturers
            .Select(x => new ManufacturerInfo
            {
                ManufacturerName = x.ManufacturerName,
                AverageInstrumentPrice = x.Instruments.Average(y => y.Price)
            })
            .ToListAsync();
    }
}
