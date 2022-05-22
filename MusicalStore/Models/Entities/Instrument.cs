namespace MusicalStore.Models.Entities;

public class Instrument : BaseEntity
{
    public string Name { get; set; }
    public int InstrumentTypeId { get; set; }
    public decimal Price { get; set; }
    public int ManufacturerId { get; set; }
    public int ManufactoringCountryId { get; set; }

    public InstrumentType InstrumentType { get; set; }
    public Country ManufactoringCountry { get; set; }
    public Manufacturer Manufacturer { get; set; }
    public ICollection<Material> Materials { get; set; } = new HashSet<Material>();
}