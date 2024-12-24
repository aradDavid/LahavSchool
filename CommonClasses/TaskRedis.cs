using API.DTOs;
using CommonClasses.Enums;
using CommonClasses.Models;

namespace CommonClasses;

public class TaskRedis
{
    public required string TaskId { get; set; }
    public required TaskType TaskName { get; set; }
    public School? SchoolData { get; set; }
    public SchoolUpdateDto? SchoolUpdateData { get; set; }
    public int SchoolId { get; set; }
    public int DistrictId { get; set; }
    public string? SchoolName { get; set; }
    
}