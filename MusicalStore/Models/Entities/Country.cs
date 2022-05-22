namespace MusicalStore.Models.Entities;

public class Country : BaseEntity
{
    public string CountryName { get; set; }

    public ICollection<Instrument> Instruments { get; set; } = new HashSet<Instrument>();

    public ICollection<Manufacturer> Manufacturers { get; set; } = new HashSet<Manufacturer>();
}