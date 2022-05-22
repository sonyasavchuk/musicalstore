using System.ComponentModel;

namespace MusicalStore.Models.Dtos.Instruments;

public class InstrumentDto
{
    public int Id { get; set; }
    
    public string Name { get; set; }

    public decimal Price { get; set; }

    [DisplayName("Type")]
    public string InstrumentType { get; set; }
    
    [DisplayName("Manufacturing Country")]
    public string ManufactoringCountryName { get; set; }
    
    [DisplayName("Manufacturer")]
    public string ManufacturerName { get; set; }
    
    public ICollection<string> Materials { get; set; } = new HashSet<string>();
}
