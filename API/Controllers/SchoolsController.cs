
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
    private readonly WorkerService _schoolService;
   
    
    public SchoolsController(myDbContext dbContext) 
    {
        _dbContext = dbContext;
        _schoolValidations = new Validations();
        _schoolService = new WorkerService();
    }

    private async Task<List<School>> getAllSchools(CancellationToken cancellationToken=default)
    {
        string taskId = Guid.NewGuid().ToString();
        await _schoolService.InsertTaskIntoQueue(taskId,TaskType.GetSchools);
        Console.WriteLine($"i'm here waiting with:{taskId}");
        var result = await _schoolService.GetTaskResults(taskId,cancellationToken);
        return result;
    }
    
    [HttpGet]
    public async Task<ActionResult<string>> GetSchools(CancellationToken cancellationToken=default)
    {
        List<School> schools =await getAllSchools(cancellationToken);
        return schools != null ? Ok(schools) : NotFound("There is no schools yet!");
        
    }
    
    [HttpGet("{id}")]
    public async Task<ActionResult<School>> GetSchoolFromId(int id,CancellationToken cancellationToken=default)
    {
        List<School> schools =await getAllSchools(cancellationToken);
        School? currSchool = null;
        foreach (School school in schools)
        {
            if (school.Id == id)
            {
                currSchool = school;
                break;
            }
        }
        return currSchool != null ? Ok(currSchool) : NotFound("There is no school found!");
        
        /* string taskId = Guid.NewGuid().ToString();
         TaskData taskData = new TaskData()
         {
             FiledName = "Id",
             Value = id.ToString()
         };
         await _schoolService.InsertTaskIntoQueue(taskId, TaskType.GetSchoolFromId,taskData);
         var result = await _schoolService.GetTaskOneResult<School>(taskId,cancellationToken);
         return result != null ? Ok(result) : NotFound("There is no school found!");
         */
    }

    [HttpGet("name/{schoolName}")]
    public async Task<ActionResult<School>> GetSchoolFromName(string schoolName,CancellationToken cancellationToken=default)
    {
        
        /*
        string taskId = Guid.NewGuid().ToString();
        TaskData taskData = new TaskData()
        {
            FiledName = "Name",
            Value =schoolName
        };
        await _schoolService.InsertTaskIntoQueue(taskId, TaskType.GetSchoolByName, taskData);
        var result = await _schoolService.GetTaskOneResult<School>(taskId,cancellationToken);
        return result != null ? Ok(result) : NotFound("There is no school found!");
        
        */
        List<School> schools =await getAllSchools(cancellationToken);
        School? currSchool = null;
        foreach (School school in schools)
        {
            if (school.Name == schoolName)
            {
                currSchool = school;
                break;
            }
        }
        return currSchool != null ? Ok(currSchool) : NotFound("There is no school found!");
    }
    
    [HttpGet("district/{districtId}")]
    public async Task<ActionResult<List<School>>> GetSchoolsFromDistrict(int districtId,CancellationToken cancellationToken=default)
    {
        
        var schools = await getAllSchools();
        List<School> allSchoolsFromDistrict = new List<School>();
        foreach (var school in schools)
        {
            if (school.DistrictId == districtId)
            {
                ValidationDisplay validationTest = _schoolValidations.CheckIdValidations(school);
                if (validationTest.IsValid)
                {
                    allSchoolsFromDistrict.Add(school);
                }
               
            }
        }
        return allSchoolsFromDistrict.Count > 0 ? Ok(allSchoolsFromDistrict) : BadRequest("There are no schools in this district!");
    }

    [HttpPost("addSchool")]
    public async Task<ActionResult<string>> AddSchool([FromQuery]SchoolDto schoolDto,CancellationToken cancellationToken=default)
    {
        bool hasDone = false;
        School newSchool = new School()
        {
            Name = schoolDto.Name,
            DistrictId = schoolDto.DistrictId,
            LicenseId = schoolDto.LicenseId,
            CreatedAt = DateTime.Now.ToString(),
        };
        PropertyInfo[] props = newSchool.GetType().GetProperties();
        foreach (var type  in props )
        {
            Console.WriteLine(type.GetValue(newSchool));
        }
            string taskId = Guid.NewGuid().ToString();
            await _schoolService.InsertTaskIntoQueue(taskId, TaskType.AddNewSchool,newSchool);
            hasDone = await _schoolService.CheckIfTaskHasCompleted(taskId);
        
        return hasDone ? Ok($"{newSchool.Name} has been inserted!!") : BadRequest("There was an error!");
        
    }
    
    [HttpGet("deleteSchool/{schoolId}")]
    public async Task<ActionResult<string>> DeleteSchool(int schoolId,CancellationToken cancellationToken=default)
    {
        bool hasDone = false;
        string taskId = Guid.NewGuid().ToString();
        await _schoolService.InsertTaskIntoQueue(taskId, TaskType.DeleteSchool,schoolId);
        hasDone = await _schoolService.CheckIfTaskHasCompleted(taskId);
        return hasDone ? Ok($"{schoolId} has been Deleted!!") : BadRequest("There was an error!");
    }
    
    
    [HttpPost("updateSchool/{schoolId}")]
        public async Task<ActionResult<string>> UpdateSchool([FromQuery]SchoolUpdateDto schoolUpdateDto,int schoolId,CancellationToken cancellationToken=default)
        {
            
            bool hasDone = false;
            string taskId = Guid.NewGuid().ToString();
            await _schoolService.InsertTaskIntoQueue(taskId, TaskType.UpdateSchool,schoolId,schoolUpdateDto);
            hasDone = await _schoolService.CheckIfTaskHasCompleted(taskId);
            return hasDone ? Ok($"{schoolId} has been Updated!!") : BadRequest("There was an error!");
            
        }
        
}