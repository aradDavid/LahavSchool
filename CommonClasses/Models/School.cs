
namespace CommonClasses.Models;


public class School
{
    public int Id { get; set; } = 0;
    public  required string Name { get; set; } = string.Empty;
    public required int DistrictId { get; set; } = 0;
    public required int LicenseId { get; set; } = 0;
    public string? CreatedAt { get; set; }
    public string? UpdatedAt { get; set; }
    public string? ExpiresAt { get; set; }
}