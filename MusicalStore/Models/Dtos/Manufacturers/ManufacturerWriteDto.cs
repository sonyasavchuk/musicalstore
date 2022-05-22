using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace MusicalStore.Models.Dtos.Manufacturers;

public class ManufacturerWriteDto
{
    [DisplayName("Name")]
    [Required]
    [StringLength(50, MinimumLength = 2)]
    public string ManufacturerName { get; set; }
    
    [DisplayName("Country")]
    [Required]
    public int CountryId { get; set; }
}
