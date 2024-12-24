
using System.Reflection;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CommonClasses.Models;
using CommonClasses.Data;
using API.DTOs;
using API.Services;
using Microsoft.AspNetCore.Http.HttpResults;
using CommonClasses;
using CommonClasses.Enums;

namespace API.Controllers;

[ApiController]
[Route("/[controller]")]
public class SchoolsController : ControllerBase
{
    private readonly myDbContext _dbContext;
    private  Validations _schoolValidations;
    private readonly SchoolWorkerService _schoolService;
  
    
    public SchoolsController(myDbContext dbContext,ILogger<SchoolsController> logger) 
    {
        _dbContext = dbContext;
        _schoolValidations = new Validations();
        _schoolService = new SchoolWorkerService();
       
    }
    
    [HttpGet]
    public async Task<ActionResult<string>> GetSchools(CancellationToken cancellationToken=default)
    {
        string taskId = await _schoolService.InsertTaskIntoQueueAsync(TaskType.GetSchools);
        var results = await _schoolService.GetTaskResults(taskId,cancellationToken);
        Console.WriteLine($"i'm here waiting with:{taskId}");
        return results.Count > 0 ? Ok(results) : NotFound("There is no schools yet!");
        
    }
    
    [HttpGet("{id}")]
    public async Task<IActionResult> GetSchoolFromId(int id,CancellationToken cancellationToken=default)
    {
        ValidationDisplay validationDisplay = _schoolValidations.CheckNumericFieldValidations(id,"Id");
        if (!validationDisplay.IsValid)
        {
            return BadRequest("There is no school with this id!");
        }
        string taskId = await _schoolService.InsertTaskIntoQueueAsync(id,TaskType.GetSchoolFromId);
        var result = await _schoolService.GetTaskResults(taskId,cancellationToken);
        return result.Count > 0 ? Ok(result) : NotFound("There is no school found!");

    }

    [HttpGet("name/{schoolName}")]
    public async Task<ActionResult<School>> GetSchoolFromName(string schoolName,CancellationToken cancellationToken=default)
    {
        string taskId = await _schoolService.InsertTaskIntoQueueAsync(schoolName);
        var result = await _schoolService.GetTaskResults(taskId,cancellationToken);
        return result.Count > 0 ? Ok(result) : NotFound("There is no school found!");
    }
    
    [HttpGet("district/{districtId}")]
    public async Task<IActionResult> GetSchoolsFromDistrict(int districtId,CancellationToken cancellationToken=default)
    {
        ValidationDisplay validationDisplay = _schoolValidations.CheckNumericFieldValidations(districtId, "DistrictId");
        if (!validationDisplay.IsValid)
        {
            return BadRequest(validationDisplay);
        }
        string taskId = await _schoolService.InsertTaskIntoQueueAsync(districtId, TaskType.GetSchoolByDistrict);
        var result = await _schoolService.GetTaskResults(taskId,cancellationToken);
        return result.Count > 0 ? Ok(result) : NotFound("There is no schools found in this district!");
        


    }

    [HttpPost("addSchool")]
    public async Task<IActionResult> AddSchool([FromQuery]SchoolDto schoolDto,CancellationToken cancellationToken=default)
    {
       
        var validationDisplays = _schoolValidations.CheckNewSchool(schoolDto);
        if (validationDisplays != null)
        {
            return BadRequest(validationDisplays);
        }
        
        School newSchool = new School()
        {
                Name = schoolDto.Name,
                DistrictId = schoolDto.DistrictId,
                LicenseId = schoolDto.LicenseId,
                CreatedAt = DateTime.Now.ToString(),
        };
        string taskId = await _schoolService.InsertTaskIntoQueueAsync(newSchool,TaskType.AddNewSchool);
        var result = await _schoolService.CheckIfTaskHasCompleted(taskId);
        return result ? Ok("School has been Added!") : BadRequest("There was an error adding school");
    }
    
    [HttpGet("deleteSchool/{schoolId}")]
    public async Task<IActionResult> DeleteSchool(int schoolId,CancellationToken cancellationToken=default)
    { 
        ValidationDisplay validationDisplay = _schoolValidations.CheckNumericFieldValidations(schoolId, "Id");
        if (!validationDisplay.IsValid)
        {
            return BadRequest("Id must be greater than 0");
        }

        string taskId = await _schoolService.InsertTaskIntoQueueAsync(schoolId, TaskType.DeleteSchool);
        var result = await _schoolService.CheckIfTaskHasCompleted(taskId);
        return result ? Ok("this school has been deleted!") : BadRequest("This school doesnt exist");
        
    }
    
    
    [HttpPost("updateSchool/{schoolId}")]
    public async Task<IActionResult> UpdateSchool([FromQuery]SchoolUpdateDto schoolUpdateDto,int schoolId,CancellationToken cancellationToken=default)
    {
        var validations = _schoolValidations.CheckUpdatedSchool(schoolUpdateDto, schoolId);
        if (validations.Count > 0)
        {
            return BadRequest(validations);
        }
        string taskId = await _schoolService.InsertTaskIntoQueueAsync(schoolId,schoolUpdateDto, TaskType.UpdateSchool);
        var result = await _schoolService.CheckIfTaskHasCompleted(taskId);
        return result? Ok("School has been updated!") : BadRequest("This School doesnt exist");
        
    }
        
}