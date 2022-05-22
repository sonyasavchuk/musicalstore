namespace MusicalStore.Models.Entities;

public class InstrumentType : BaseEntity
{
    public string TypeName { get; set; }
    public int GroupId { get; set; }

    public Group Group { get; set; }
    public ICollection<Instrument> Instruments { get; set; } = new HashSet<Instrument>();
}