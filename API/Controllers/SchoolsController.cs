
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using API.Models;
using System.IO;
using System.Text.Json;

namespace API.Controllers;

[ApiController]
[Route("/[controller]")]
public class SchoolsController : ControllerBase
{
    private string m_Path = Directory.GetCurrentDirectory();
    
    private async Task<List<SchoolModel>> getAllSchoolsFromDb()
    {
        string dbName = m_Path +"/schools.json";
        string jsonStr = String.Empty;
        using (StreamReader sr = new StreamReader(dbName))
        {
            jsonStr = await sr.ReadToEndAsync();
            List<SchoolModel> schools = JsonSerializer.Deserialize<List<SchoolModel>>(jsonStr);
            return schools;
        }
        
    }
    
    [HttpGet]
    public async Task<ActionResult<string>> GetSchools()
    {
        List<SchoolModel> schools = await getAllSchoolsFromDb();
        return schools.Count >0 ? Ok(schools) : BadRequest("There are no schools"); 
    }

    [HttpGet("{i_id}")]
    public async Task<ActionResult<SchoolModel>> GetSchoolById(int i_id)
    {
        List<SchoolModel> schools = await getAllSchoolsFromDb();
        bool isFoundSchool = false;
        SchoolModel schoolAcordId = new SchoolModel
        {
            Id = 0,
            SchoolName = string.Empty,
            DistrictId = 0,
            LicenseId = 0
        };
        foreach (var school in schools)
        {
            if (school.Id == i_id)
            {
                schoolAcordId = school;
                isFoundSchool = true;
                break;

            }
        }
        
        return isFoundSchool? Ok(schoolAcordId) : BadRequest("There is no School with this Id!!");
            
    }
    
    [HttpGet("District/{i_DisId}")]
    public async Task<ActionResult<SchoolModel>> GetSchoolByDistrictId(int i_DisId)
    {
        List<SchoolModel> schools = await getAllSchoolsFromDb();
        List<SchoolModel> schoolsByDis = new List<SchoolModel>();
        foreach (var school in schools)
        {
            if (school.DistrictId == i_DisId)
            {
                schoolsByDis.Add(school);
            }
        }
        return schoolsByDis.Count>0 ? Ok(schools) : BadRequest("There is no schools in this District");
            
    }
    
   /* [HttpPost("Update")]
    public async Task<ActionResult<bool>> UpdateSchool()
    {
        
    }*/
}