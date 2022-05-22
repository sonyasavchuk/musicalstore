namespace MusicalStore.Models.Entities;

public class InstrumentMaterial : BaseEntity
{
    public int InstrumentId { get; set; }
    public int MaterialId { get; set; }

    public Instrument Instrument { get; set; }
    public Material Material { get; set; }
}