using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace MusicalStore.Models.Dtos.Groups;

public class GroupWriteDto
{
    [DisplayName("Group")]
    [Required]
    [StringLength(50, MinimumLength = 2)]
    public string GroupName { get; set; }

    [DisplayName("Types")]
    [Required]
    public IEnumerable<string> Types { get; set; } = new HashSet<string>();
}
