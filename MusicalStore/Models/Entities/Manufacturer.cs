namespace MusicalStore.Models.Entities;

public class Manufacturer : BaseEntity
{
    public string ManufacturerName { get; set; }
    public int CountryId { get; set; }

    public Country Country { get; set; }
    public ICollection<Instrument> Instruments { get; set; } = new HashSet<Instrument>();
}