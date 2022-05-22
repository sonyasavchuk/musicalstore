namespace MusicalStore.Models.Dtos.Instruments;

public class InstrumentExternalDto
{
    public string Name { get; set; }
    
    public string InstrumentTypeName { get; set; }
    
    public decimal Price { get; set; }
    
    public string ManufacturerName { get; set; }
    
    public string ManufactoringCountryName { get; set; }

    public IEnumerable<string> Materials { get; set; } = new List<string>();
}
