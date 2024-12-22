
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
    private readonly myDbContext m_DbContext;
    
    public SchoolsController(myDbContext i_DbContext) 
    {
        m_DbContext = i_DbContext;
    }
    
    [HttpGet]
    public async Task<ActionResult<string>> GetSchools()
    {
        var schools = await m_DbContext.Schools.ToListAsync();
        return Ok(schools);
    }
    
    [HttpGet("{i_id}")]
    public async Task<ActionResult<School>> GetSchoolFromId(int i_id)
    {
        var school = await m_DbContext.Schools.FindAsync(i_id);
        return school != null ? Ok(school) : BadRequest("There is no schools with this id!");
    }

    [HttpGet("name/{i_SchoolName}")]
    public async Task<ActionResult<School>> GetSchoolFromName(string i_SchoolName)
    {
        var school = await m_DbContext.Schools.FirstOrDefaultAsync(school => school.Name == i_SchoolName);
        return school != null ? Ok(school) : BadRequest("There is no school with name!");
    }

    [HttpGet("district/{i_DistrictName}")]
    public async Task<ActionResult<List<School>>> GetSchoolsFromDistrict(int i_DistrictName)
    {
        var schools = await m_DbContext.Schools.ToListAsync();
        List<School> allSchoolsFromDistrict = new List<School>();
        foreach (var school in schools)
        {
            if (school.DistrictId == i_DistrictName)
            {
                allSchoolsFromDistrict.Add(school);
            }
        }
        return schools.Count > 0 ? Ok(allSchoolsFromDistrict) : BadRequest("There are no schools in this district!");
    }

    [HttpPost("addSchool")]
    public async Task<ActionResult<string>> AddSchool([FromQuery]SchoolDto i_SchoolDto)
    {
        School newSchool = new School()
        {
            Name = i_SchoolDto.Name,
            DistrictId = i_SchoolDto.DistrictId,
            LicenseId = i_SchoolDto.LicenseId,
            CreatedAt = DateTime.Now.ToString(),
            UpdatedAt = DateTime.Now.ToString(),
            ExpiresAt = DateTime.Now.AddYears(1).ToString()
            
        };
        m_DbContext.Schools.Add(newSchool);
        await m_DbContext.SaveChangesAsync();
        return Ok("School added successfully!");
    }
    
    [HttpGet("deleteSchool/{i_schoolId}")]
    public async Task<ActionResult<string>> DeleteSchool(int i_schoolId)
    {
        ActionResult<string> result;
        var deletedSchool = await m_DbContext.Schools.FindAsync(i_schoolId);
        if (deletedSchool != null)
        {
            m_DbContext.Schools.Remove(deletedSchool);
            await m_DbContext.SaveChangesAsync();
            result = Ok($"{deletedSchool.Name} has been deleted!");
        }
        else
        {
            result = BadRequest("There is no school with that id!");
        }

        return result;
    }

    [HttpPost("updateSchool/{i_schoolId}")]
        public async Task<ActionResult<string>> UpdateSchool([FromQuery]SchoolUpdateDto i_SchoolUpdateDto,int i_schoolId)
        {
            ActionResult<string> result = BadRequest("There is no school with that id!");
            var currentSchool = await m_DbContext.Schools.FindAsync(i_schoolId);
            if (currentSchool != null)
            {
                if (i_SchoolUpdateDto.Name != null )
                {
                    currentSchool.Name = i_SchoolUpdateDto.Name;
                }
    
                if (i_SchoolUpdateDto.DistrictId >=0)
                {
                    currentSchool.DistrictId = i_SchoolUpdateDto.DistrictId;
                }
    
                if (i_SchoolUpdateDto.LicenseId >=0)
                {
                    currentSchool.LicenseId = i_SchoolUpdateDto.LicenseId;
                }
    
                if (i_SchoolUpdateDto.ExpiredAt != string.Empty)
                {
                    currentSchool.ExpiresAt = i_SchoolUpdateDto.ExpiredAt;
                }
                
                currentSchool.UpdatedAt = DateTime.Now.ToString();
                m_DbContext.Schools.Update(currentSchool);
                await m_DbContext.SaveChangesAsync();
                result = Ok($"{currentSchool.Name} has been updated!");
            }
    
            return result;
        }
        
    
    
}