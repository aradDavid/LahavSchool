using System.Reflection;
using System.Text.Json;
using API.DTOs;
using CommonClasses.Data;
using CommonClasses;
using CommonClasses.Enums;
using CommonClasses.Models;
using Microsoft.EntityFrameworkCore;

namespace ms_db.Services;

public class WorkerSubServices : WorkerBase
{
    private readonly myDbContext _dbContext;

    public WorkerSubServices(myDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    private async Task getSchools(string taskId)
    {
        var allSchools = await _dbContext.Schools.ToListAsync();
        foreach (var school in allSchools)
        {
            Console.WriteLine(school.Name);
        }
        var allSchoolsAsJson = JsonSerializer.Serialize(allSchools);
        Console.WriteLine($"Pushing the result with id:{taskId}");
        Console.WriteLine(allSchoolsAsJson);
        await _db.StringSetAsync(taskId, allSchoolsAsJson);
        string statusKey = "status" + taskId;
        await _db.StringSetAsync(statusKey, "Done");
    }

    private async Task getSchoolById(string taskId,int schoolId)
    {
        var currSchool = await _dbContext.Schools.FindAsync(schoolId);
        
        var currSchoolAsJson = JsonSerializer.Serialize(currSchool);
        Console.WriteLine("Found it :"+currSchoolAsJson);
        await _db.StringSetAsync(taskId, currSchoolAsJson);
        string statusKey = "status" + taskId;
        await _db.StringSetAsync(statusKey, "Done");
    }
    
    private async Task getSchoolByName(string taskId,string schoolName)
    {
        var currSchool = await _dbContext.Schools.FindAsync(schoolName);
        var currSchoolAsJson = JsonSerializer.Serialize(currSchool);
        Console.WriteLine("Found it :"+currSchoolAsJson);
        await _db.StringSetAsync(taskId, currSchoolAsJson);
        string statusKey = "status" + taskId;
        await _db.StringSetAsync(statusKey, "Done");
    }

    private async Task addNewSchool(string taskId,School newSchool)
    {
        /*
        PropertyInfo[] props = newSchool.GetType().GetProperties();
        foreach (var type  in props )
        {
            Console.WriteLine(type.PropertyType.Name);
            Console.WriteLine(type.GetValue(newSchool));
        }*/
        _dbContext.Schools.Add(newSchool);
        await _dbContext.SaveChangesAsync();
        string statusKey = "status" + taskId;
        await _db.StringSetAsync(statusKey, "Done");
    }

    private async Task deleteSchool(string taskId,int schoolId)
    {
        var deletedSchool = await _dbContext.Schools.FindAsync(schoolId);
        deletedSchool.ExpiresAt = DateTime.Now.ToString();
        await _dbContext.SaveChangesAsync();
        string statusKey = "status" + taskId;
        await _db.StringSetAsync(statusKey, "Done");
    }

    private async Task UpdateSchool(string taskId, int schooldId,SchoolUpdateDto updatedSchoolDto)
    {
        var updatedSchool = await _dbContext.Schools.FindAsync(schooldId);
        
        if (updatedSchoolDto.Name != null )
        {
            updatedSchool.Name = updatedSchoolDto.Name;
        }
    
        if (updatedSchoolDto.DistrictId >= 0)
        {
            updatedSchool.DistrictId = updatedSchoolDto.DistrictId;
        }
    
        if (updatedSchoolDto.LicenseId >= 0)
        {
            updatedSchool.LicenseId = updatedSchoolDto.LicenseId;
        }
    
        if (updatedSchoolDto.ExpiredAt != string.Empty)
        {
            updatedSchool.ExpiresAt = updatedSchoolDto.ExpiredAt;
        }
                
        updatedSchool.UpdatedAt = DateTime.Now.ToString();
        _dbContext.Schools.Update(updatedSchool);
        await _dbContext.SaveChangesAsync();
        string statusKey = "status" + taskId;
        await _db.StringSetAsync(statusKey, "Done");
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
                TaskType taskAction = taskData.TaskName;
                switch (taskAction)
                {
                    case TaskType.GetSchools:
                    {
                        Console.WriteLine("Getting all schools!");
                        await getSchools(taskId);
                        break;
                    }
                    case TaskType.GetSchoolFromId:
                    {
                        /*TaskData schoolId = taskData.TaskData;
                        Console.WriteLine("Getting school from id!");
                        await getSchoolById(taskId,int.Parse(schoolId.Value));*/
                        break;
                    }
                    case TaskType.GetSchoolByName:
                        /*TaskData schoolName = taskData.TaskData;*/
                        break;
                    case TaskType.AddNewSchool:
                        await addNewSchool(taskId, taskData.SchoolData);
                        break;
                    case TaskType.DeleteSchool:
                        await deleteSchool(taskId, taskData.SchoolId);
                        break;
                    case TaskType.UpdateSchool:
                        await UpdateSchool(taskId, taskData.SchoolId, taskData.SchoolUpdateData);
                        break;
                }

               
            }

           // await Task.Delay(1000);
        }
    }
   
}