namespace MusicalStore.Models.Entities;

public class Material : BaseEntity
{
    public string MaterialName { get; set; }

    public ICollection<Instrument> Instruments { get; set; } = new HashSet<Instrument>();
}