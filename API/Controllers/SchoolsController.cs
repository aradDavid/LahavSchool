
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
   
    
    public SchoolsController(myDbContext dbContext) 
    {
        _dbContext = dbContext;
        _schoolValidations = new Validations();
        _schoolService = new SchoolWorkerService();
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
        ActionResult<School> finalResult = BadRequest("There is no school with this id!");
        List<School> schools = new List<School>();
        ValidationDisplay validationDisplay = _schoolValidations.CheckNumericFieldValidations(id,"Id");
        
        if (validationDisplay.IsValid)
        {
            schools =await getAllSchools(cancellationToken);
            foreach (School school in schools)
            {
                if (school.Id == id)
                {
                    finalResult = Ok(school);
                    break;
                }
            }
        }
        else
        {
            finalResult = BadRequest(validationDisplay.ErrorMessage);
        }

        return finalResult;

    }

    [HttpGet("name/{schoolName}")]
    public async Task<ActionResult<School>> GetSchoolFromName(string schoolName,CancellationToken cancellationToken=default)
    {
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
        ActionResult<List<School>> finalResult = NotFound("There is no schools with this district!");
        List<School> allSchoolsFromDistrict = new List<School>();
        ValidationDisplay validationDisplay = _schoolValidations.CheckNumericFieldValidations(districtId, "DistrictId");
        if (validationDisplay.IsValid)
        {
            var schools = await getAllSchools(cancellationToken);
            foreach (var school in schools)
            {
                if (school.DistrictId == districtId)
                {
                    allSchoolsFromDistrict.Add(school);
                }
            }
        }
        else
        {
            finalResult = BadRequest(validationDisplay.ErrorMessage);
        }

        if (allSchoolsFromDistrict.Count > 0)
        {
            finalResult = Ok(allSchoolsFromDistrict);
        }
        return finalResult;
    }

    [HttpPost("addSchool")]
    public async Task<ActionResult<string>> AddSchool([FromQuery]SchoolDto schoolDto,CancellationToken cancellationToken=default)
    {
        ActionResult<string> finalReslt = BadRequest("There was an error inserting the school!");
        List<ValidationDisplay>? validationDisplays = _schoolValidations.CheckNewSchool(schoolDto);
        if (validationDisplays == null)
        {
            
            School newSchool = new School()
            {
                Name = schoolDto.Name,
                DistrictId = schoolDto.DistrictId,
                LicenseId = schoolDto.LicenseId,
                CreatedAt = DateTime.Now.ToString(),
            };
            string taskId = Guid.NewGuid().ToString();
            await _schoolService.InsertTaskIntoQueue(taskId, TaskType.AddNewSchool,newSchool);
            if(await _schoolService.CheckIfTaskHasCompleted(taskId))
            {
                finalReslt = Ok($"{newSchool.Name} was inserted successfully!");
            }
        }
        else
        {
            finalReslt = BadRequest(validationDisplays); 
        }


        return finalReslt;

    }
    
    [HttpGet("deleteSchool/{schoolId}")]
    public async Task<ActionResult<string>> DeleteSchool(int schoolId,CancellationToken cancellationToken=default)
    {
        ValidationDisplay validationDisplay = _schoolValidations.CheckNumericFieldValidations(schoolId, "Id");
        ActionResult<string> finalResult = BadRequest("There was an error deleting the school!");
        if (validationDisplay.IsValid)
        {
            bool hasDone = false;
            string taskId = Guid.NewGuid().ToString();
            await _schoolService.InsertTaskIntoQueue(taskId, TaskType.DeleteSchool,schoolId);
            if (await _schoolService.CheckIfTaskHasCompleted(taskId))
            {
                finalResult = Ok($"{schoolId} was deleted successfully!");
            }
            
        }
        return finalResult;

    }
    
    
    [HttpPost("updateSchool/{schoolId}")]
    public async Task<ActionResult<List<ValidationDisplay>>> UpdateSchool([FromQuery]SchoolUpdateDto schoolUpdateDto,int schoolId,CancellationToken cancellationToken=default)
    {
            ActionResult<List<ValidationDisplay>> result = BadRequest("There was an error updating the school!");
            List<ValidationDisplay> validations = _schoolValidations.CheckUpdatedSchool(schoolUpdateDto, schoolId);
            if (validations.Count == 0)
            {
                var allSchools = await getAllSchools(cancellationToken);
                foreach (var school in allSchools)
                {
                    if (school.Id == schoolId)
                    {
                        _schoolValidations.CheckEmptyFields(schoolUpdateDto,school);
                    }
                }
                
                string taskId = Guid.NewGuid().ToString();
                await _schoolService.InsertTaskIntoQueue(taskId, TaskType.UpdateSchool,schoolId,schoolUpdateDto);
                if (await _schoolService.CheckIfTaskHasCompleted(taskId))
                {
                    result = Ok($"{schoolId} was updated successfully!");
                }
               
            }
            else
            {
                result = BadRequest(validations);
            }

            return result;

    }
        
}