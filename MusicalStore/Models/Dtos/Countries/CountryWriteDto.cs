using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace MusicalStore.Models.Dtos.Countries;

public class CountryWriteDto
{
    [DisplayName("Name")]
    [StringLength(50, MinimumLength = 2)]
    public string CountryName { get; set; }
}
