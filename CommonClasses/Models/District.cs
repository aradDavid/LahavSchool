namespace CommonClasses.Models;

public class District
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public string? CreatedAt { get; set; }
    public string? UpdatedAt { get; set; }
    public string? ExpiredAt { get; set; }
    
}
