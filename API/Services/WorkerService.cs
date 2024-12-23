
using System.Reflection;
using System.Text.Json;
using API.DTOs;
using Microsoft.AspNetCore.Mvc;
using CommonClasses;
using CommonClasses.Enums;
using CommonClasses.Models;

namespace API.Services;

public class WorkerService : WorkerBase
{
    public async Task<bool> CheckIfTaskHasCompleted(string taskId)
    {
        int tries = 5;
        string? taskResult = string.Empty;
        while (tries >= 0 || taskResult == "Done")
        {
             taskResult = await _db.StringGetAsync(taskId);
             tries--;
             await Task.Delay(200);
        }

        return true;
    }
    /*
    public async Task InsertTaskIntoQueue(string taskId,TaskType taskName,TaskData taskData)
    {
        TaskRedis currTask = new TaskRedis()
        {
            TaskId = taskId,
            TaskName = taskName,
            TaskData = taskData
        };
        await _db.ListRightPushAsync("TasksQueue", JsonSerializer.Serialize(currTask));
       
    }*/
     public async Task InsertTaskIntoQueue(string taskId,TaskType taskName,School taskData)
    {
        TaskRedis currTask = new TaskRedis()
        {
            TaskId = taskId,
            TaskName = taskName,
        };
        if (taskData != null)
        {
            currTask.SchoolData = taskData;
            PropertyInfo[] props = currTask.SchoolData.GetType().GetProperties();
            foreach (var type  in props )
            {
                Console.WriteLine(type.GetValue(currTask.SchoolData));
            }
        }
        
        await _db.ListRightPushAsync("TasksQueue", JsonSerializer.Serialize(currTask));
       
    } 
     
    public async Task InsertTaskIntoQueue(string taskId,TaskType taskName)
    {
        TaskRedis currTask = new TaskRedis()
        {
            TaskId = taskId,
            TaskName = taskName,
        };
        
        await _db.ListRightPushAsync("TasksQueue", JsonSerializer.Serialize(currTask));
       
    } 
    
    
    public async Task InsertTaskIntoQueue(string taskId,TaskType taskName,int schoolId)
    {
        TaskRedis currTask = new TaskRedis()
        {
            TaskId = taskId,
            TaskName = taskName,
            SchoolId = schoolId
        };
        
        await _db.ListRightPushAsync("TasksQueue", JsonSerializer.Serialize(currTask));
       
    } 
    
    public async Task InsertTaskIntoQueue(string taskId,TaskType taskName,int schoolId,SchoolUpdateDto schoolUpdate)
    {
        TaskRedis currTask = new TaskRedis()
        {
            TaskId = taskId,
            TaskName = taskName,
            SchoolId = schoolId,
            SchoolUpdateData = schoolUpdate
        };
        
        await _db.ListRightPushAsync("TasksQueue", JsonSerializer.Serialize(currTask));
       
    } 

    public async Task<List<School>> GetTaskResults(string taskId, CancellationToken cancellationToken)
    {
        
        List<School> returnState = null;
        if ( await CheckIfTaskHasCompleted("stauts" + taskId))
        {
            Console.WriteLine("I'm here");
            Console.WriteLine(taskId);
            var taskResult = await _db.StringGetAsync(taskId);
            Console.WriteLine(taskResult);
            if (!taskResult.IsNullOrEmpty)
            {
                returnState = JsonSerializer.Deserialize<List<School>>(taskResult);

            }
        }
        
        return returnState;
    }
    
    public async Task<T> GetTaskOneResult<T>(string taskId, CancellationToken cancellationToken)
    {

        T returnState = default;
        if ( await CheckIfTaskHasCompleted("stauts" + taskId))
        {
            Console.WriteLine("I'm here");
            Console.WriteLine(taskId);
            var taskResult = await _db.StringGetAsync(taskId);
            Console.WriteLine(taskResult);
            if (!taskResult.IsNullOrEmpty)
            {
                returnState = JsonSerializer.Deserialize<T>(taskResult);
            }
        }
        
        return returnState;
    }
}