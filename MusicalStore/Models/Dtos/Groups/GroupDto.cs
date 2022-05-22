namespace MusicalStore.Models.Dtos.Groups;

public class GroupDto
{
    public int Id { get; set; }
    
    public string GroupName { get; set; }
    
    public IEnumerable<string> Types { get; set; }
}
