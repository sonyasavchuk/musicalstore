namespace MusicalStore.Models.ExternalData;

public class CommonTable
{
    public string Title { get; set; } = string.Empty;
    
    public List<Column> Columns { get; set; } = new();
    public List<Row> Rows { get; set; } = new();

    public bool Validate()
    {
        if (Rows.Any(x => x.Cells.Count != Columns.Count))
        {
            return false;
        }
        
        return true;
    }
    
    public record Column(string Title);

    public class Row
    {
        public List<string> Cells { get; set; } = new();
    }
}
