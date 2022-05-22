using System.ComponentModel;

namespace MusicalStore.Models.Dtos.Countries;

public class CountryDto
{
    public int Id { get; set; }
    
    [DisplayName("Name")]
    public string CountryName { get; set; }
    
    [DisplayName("# Instruments")]
    public int InstrumentsCount { get; set; }
    
    [DisplayName("# Manufacturers")]
    public int ManufacturersCount { get; set; }
}
