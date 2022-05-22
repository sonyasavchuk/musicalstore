using System.ComponentModel;

namespace MusicalStore.Models.Dtos.Manufacturers;

public class ManufacturerDto
{
    public int Id { get; set; }
    
    [DisplayName("Name")]
    public string ManufacturerName { get; set; }
    
    [DisplayName("Country name")]
    public string CountryName { get; set; }
    
    [DisplayName("# Instruments")]
    public int InstrumentsCount { get; set; }
}
