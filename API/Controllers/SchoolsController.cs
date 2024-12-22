
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using API.Models;
using API.Data;
using API.DTOs;

namespace API.Controllers;

[ApiController]
[Route("/[controller]")]
public class SchoolsController : ControllerBase
{
    private readonly myDbContext _dbContext;
    private readonly Validations _validations;
    
    public SchoolsController(myDbContext dbContext) 
    {
        _dbContext = dbContext;
        _validations = new Validations();
    }
    
    [HttpGet]
    public async Task<ActionResult<string>> GetSchools(CancellationToken cancellationToken=default)
    {
        var schools = await _dbContext.Schools.ToListAsync(cancellationToken);
        return Ok(schools);
    }
    
    [HttpGet("{id}")]
    public async Task<ActionResult<School>> GetSchoolFromId(int id,CancellationToken cancellationToken=default)
    {
        var school = await _dbContext.Schools.FindAsync(id,cancellationToken);
        ValidationDisplay validationTest = _validations.CheckIdValidations(school);
        return validationTest.IsValid  ? Ok("School has been added!") : BadRequest(validationTest);
    }

    [HttpGet("name/{schoolName}")]
    public async Task<ActionResult<School>> GetSchoolFromName(string schoolName,CancellationToken cancellationToken=default)
    {
        var school = await _dbContext.Schools.FirstOrDefaultAsync(school => school.Name == schoolName,cancellationToken);
        ValidationDisplay validationTest = _validations.CheckSchoolNameValidations(school);
        return validationTest.IsValid ? Ok(school) : BadRequest(validationTest);
    }

    [HttpGet("district/{districtName}")]
    public async Task<ActionResult<List<School>>> GetSchoolsFromDistrict(int districtName,CancellationToken cancellationToken=default)
    {
        var schools = await _dbContext.Schools.ToListAsync(cancellationToken);
        List<School> allSchoolsFromDistrict = new List<School>();
        foreach (var school in schools)
        {
            if (school.DistrictId == districtName)
            {
                allSchoolsFromDistrict.Add(school);
            }
        }
        return schools.Count > 0 ? Ok(allSchoolsFromDistrict) : BadRequest("There are no schools in this district!");
    }

    [HttpPost("addSchool")]
    public async Task<ActionResult<string>> AddSchool([FromQuery]SchoolDto schoolDto,CancellationToken cancellationToken=default)
    {
        ActionResult<string> result;
        School newSchool = new School()
        {
            Name = schoolDto.Name,
            DistrictId = schoolDto.DistrictId,
            LicenseId = schoolDto.LicenseId,
            CreatedAt = DateTime.Now.ToString(),
        };
        List<ValidationDisplay> validationTest = _validations.CheckNewSchool(newSchool);
        if (validationTest == null)
        {
            _dbContext.Schools.Add(newSchool);
            await _dbContext.SaveChangesAsync(cancellationToken);
            result = Ok("School has been added!");
        }
        else
        {
            result = BadRequest(validationTest);
        }

        return result;
    }
    
    [HttpGet("deleteSchool/{schoolId}")]
    public async Task<ActionResult<string>> DeleteSchool(int schoolId,CancellationToken cancellationToken=default)
    {
        ActionResult<string> result;
        var deletedSchool = await _dbContext.Schools.FindAsync(schoolId);
        ValidationDisplay validTest = _validations.CheckDeletedSchool(deletedSchool);
        if (validTest.IsValid)
        {
            deletedSchool.ExpiresAt = DateTime.Now.ToString();
            await _dbContext.SaveChangesAsync(cancellationToken);
            result = Ok($"{deletedSchool.Name} has been deleted!");
        }
        else
        {
            result = BadRequest(validTest);
        }

        return result;
    }

    [HttpPost("updateSchool/{schoolId}")]
        public async Task<ActionResult<string>> UpdateSchool([FromQuery]SchoolUpdateDto schoolUpdateDto,int schoolId,CancellationToken cancellationToken=default)
        {
            ActionResult<string> result = BadRequest("There is no school with that id!");
            var currentSchool = await _dbContext.Schools.FindAsync(schoolId,cancellationToken);
            List<ValidationDisplay> validationsTest = _validations.CheckNewSchool(currentSchool);
            if (validationsTest.Count == 0)
            {
                if (schoolUpdateDto.Name != null )
                {
                    currentSchool.Name = schoolUpdateDto.Name;
                }
    
                if (schoolUpdateDto.DistrictId >=0)
                {
                    currentSchool.DistrictId = schoolUpdateDto.DistrictId;
                }
    
                if (schoolUpdateDto.LicenseId >=0)
                {
                    currentSchool.LicenseId = schoolUpdateDto.LicenseId;
                }
    
                if (schoolUpdateDto.ExpiredAt != string.Empty)
                {
                    currentSchool.ExpiresAt = schoolUpdateDto.ExpiredAt;
                }
                
                currentSchool.UpdatedAt = DateTime.Now.ToString();
                _dbContext.Schools.Update(currentSchool);
                await _dbContext.SaveChangesAsync(cancellationToken);
                result = Ok($"{currentSchool.Name} has been updated!");
            }
            else
            {
                result = BadRequest(validationsTest);
            }
    
            return result;
        }
        
    
    
}