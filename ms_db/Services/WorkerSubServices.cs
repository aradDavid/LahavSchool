using System.Linq;
using System.Text.Json;
using API.DTOs;
using ms_db.Data;
using CommonClasses;
using CommonClasses.Enums;
using CommonClasses.Models;
using Microsoft.EntityFrameworkCore;

namespace ms_db.Services;

public class WorkerSubServices : WorkerBase
{
    private readonly myDbContext _dbContext;
    private readonly Validations _validations;

    public WorkerSubServices(myDbContext dbContext)
    {
        _dbContext = dbContext;
        _validations = new Validations();
    }

    private async Task getSchools(string taskId)
    {
        var allSchools = from s in _dbContext.Schools where s.ExpiresAt == null select s;
        var allSchoolsAsJson = JsonSerializer.Serialize(allSchools);
        Console.WriteLine($"Pushing the result with id:{taskId}");
        Console.WriteLine(allSchoolsAsJson);
        await _db.StringSetAsync(taskId, allSchoolsAsJson);
        string statusKey = "status" + taskId;
        await _db.StringSetAsync(statusKey, "Done");
    }

    private async Task getSchoolById(string taskId,int schoolId)
    {
        var currSchoolTest = from s in _dbContext.Schools
            where s.Id == schoolId && s.ExpiresAt==null 
            select s;
        var currSchool = await _dbContext.Schools.FindAsync(schoolId);
        var currSchoolAsJson = JsonSerializer.Serialize(currSchoolTest);
        Console.WriteLine("Found it :"+currSchoolAsJson);
        await _db.StringSetAsync(taskId, currSchoolAsJson);
        string statusKey = "status" + taskId;
        await _db.StringSetAsync(statusKey, "Done");
    }
    
    private async Task getSchoolByName(string taskId,string schoolName)
    {
        var schoolNameQuery = from s in _dbContext.Schools
            where s.Name == schoolName && s.ExpiresAt == null 
            select s;
        var currSchoolAsJson = JsonSerializer.Serialize(schoolNameQuery);
        Console.WriteLine("Found it :"+currSchoolAsJson);
        await _db.StringSetAsync(taskId, currSchoolAsJson);
        string statusKey = "status" + taskId;
        await _db.StringSetAsync(statusKey, "Done");
    }
    private async Task getSchoolByDistrict(string taskId, int districtId)
    {
        var districtQuery = from s in _dbContext.Schools
            where s.DistrictId == districtId && s.ExpiresAt== null
            select s;
        var currSchoolsAsJson = JsonSerializer.Serialize(districtQuery);
        Console.WriteLine("Found it :"+currSchoolsAsJson);
        await _db.StringSetAsync(taskId, currSchoolsAsJson);
        string statusKey = "status" + taskId;
        await _db.StringSetAsync(statusKey, "Done");
    }
    private async Task addNewSchool(string taskId,School newSchool)
    {
        _dbContext.Schools.Add(newSchool);
        await _dbContext.SaveChangesAsync();
        string statusKey = "status" + taskId;
        await _db.StringSetAsync(statusKey, "Done");
    }
    private async Task deleteSchool(string taskId,int schoolId)
    {
        string statusKey = "status" + taskId;
        var deletedSchool = await _dbContext.Schools.FindAsync(schoolId);
        if (deletedSchool != null && deletedSchool.ExpiresAt == null)
        {
            deletedSchool.ExpiresAt = DateTime.Now.ToString();
            await _dbContext.SaveChangesAsync();
            await _db.StringSetAsync(statusKey, "Done");
        }
        else
        {
            await _db.StringSetAsync(statusKey,"Doesnt-Exist");
        }
       
    }

    private async Task UpdateSchool(string taskId, int schooldId,SchoolUpdateDto updatedSchoolDto)
    {
        string statusKey = "status" + taskId;
        var updatedSchool = await _dbContext.Schools.FindAsync(schooldId);
        if (updatedSchool != null && updatedSchoolDto.ExpiredAt == null)
        {
            if (!_validations.CheckIfFieldIsEmpty(updatedSchoolDto.DistrictId))
            {
                updatedSchool.DistrictId = updatedSchoolDto.DistrictId;
            }

            if (!_validations.CheckIfFieldIsEmpty(updatedSchoolDto.LicenseId))
            {
                updatedSchool.LicenseId = updatedSchoolDto.LicenseId;
            }

            if (!_validations.CheckIfFieldIsEmpty(updatedSchoolDto.Name))
            {
                updatedSchool.Name = updatedSchoolDto.Name;
            }
            updatedSchool.UpdatedAt = DateTime.Now.ToString();
            _dbContext.Schools.Update(updatedSchool);
            await _dbContext.SaveChangesAsync();
            await _db.StringSetAsync(statusKey, "Done");
        }
        else
        {
            await _db.StringSetAsync(statusKey, "Doesnt-Exist");
        }
        
    
        
                
       
       
       
       
        
    }
    
    
    public async Task PullTasks(CancellationToken cancellationToken = default)
    {
        while (!cancellationToken.IsCancellationRequested)
        {
            var taskJson = await _db.ListLeftPopAsync("TasksQueue");
            School newSchol = null;
            if (!taskJson.IsNullOrEmpty)
            {
                var taskData = JsonSerializer.Deserialize<TaskRedis>(taskJson);
                var taskId = taskData.TaskId;
                switch (taskData.TaskName)
                {
                    case TaskType.GetSchools:
                    {
                        Console.WriteLine("Getting all schools!");
                        await getSchools(taskId);
                        break;
                    }
                    case TaskType.GetSchoolFromId:
                    {
                        Console.WriteLine("Getting School by ID");
                        await getSchoolById(taskId, taskData.SchoolId);
                        break;
                    }
                    case TaskType.GetSchoolByName:
                    {
                        await getSchoolByName(taskId, taskData.SchoolName);
                        break;
                    }
                    case TaskType.GetSchoolByDistrict:
                    {
                        await getSchoolByDistrict(taskId, taskData.DistrictId);
                        break;
                    }

                    case TaskType.AddNewSchool:
                    {
                        await addNewSchool(taskId, taskData.SchoolData);
                        break;
                    }

                    case TaskType.DeleteSchool:
                    {
                        await deleteSchool(taskId, taskData.SchoolId);
                        break;
                    }

                    case TaskType.UpdateSchool:
                    {
                        await UpdateSchool(taskId, taskData.SchoolId, taskData.SchoolUpdateData);
                        break;
                    }
                        
                }

               
            }
            
        }
    }
   
}