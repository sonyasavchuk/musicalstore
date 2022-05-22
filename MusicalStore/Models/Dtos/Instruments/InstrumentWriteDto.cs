using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace MusicalStore.Models.Dtos.Instruments;

public class InstrumentWriteDto
{
    [Required]
    [StringLength(50, MinimumLength = 2)]
    public string Name { get; set; }

    [Range(0, 99999999999)]
    public decimal Price { get; set; }
    
    [DisplayName("Type")]
    public int InstrumentTypeId { get; set; }

    [DisplayName("Manufacturer")]
    public int ManufacturerId { get; set; }
    
    [DisplayName("Manufacturing Country")]
    public int ManufactoringCountryId { get; set; }
    
    [Required]
    public ICollection<string> Materials { get; set; } = new List<string>();
}
