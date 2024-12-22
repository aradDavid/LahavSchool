namespace API.DTOs;

public class SchoolUpdateDto
{
    public  string? Name { get; set; }
    public  int DistrictId { get; set; } 
    public  int LicenseId { get; set; }
    public string? ExpiredAt { get; set; }
}