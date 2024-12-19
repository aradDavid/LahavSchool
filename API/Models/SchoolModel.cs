using System.ComponentModel.DataAnnotations.Schema;

namespace API.Models;


public class SchoolModel
{
    public required int Id { get; set; } = 0;
    public  required string SchoolName { get; set; } = string.Empty;
    public required int DistrictId { get; set; } = 0;
    public required int LicenseId { get; set; } = 0;
    public string? CreatedAt { get; set; }
    public string? UpdatedAt { get; set; }
    public string? ExpiresAt { get; set; }
}