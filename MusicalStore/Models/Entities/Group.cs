namespace MusicalStore.Models.Entities;

public class Group : BaseEntity
{
    public string GroupName { get; set; }

    public ICollection<InstrumentType> Types { get; set; } = new HashSet<InstrumentType>();
}