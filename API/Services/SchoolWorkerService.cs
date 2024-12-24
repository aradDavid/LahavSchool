using System.Reflection;
using System.Text.Json;
using API.DTOs;
using Microsoft.AspNetCore.Mvc;
using CommonClasses;
using CommonClasses.Enums;
using CommonClasses.Models;

namespace API.Services;

public class SchoolWorkerService : WorkerBase
{
    public async Task<bool> CheckIfTaskHasCompleted(string taskId)
    {
        int tries = 5;
        string? taskResult = string.Empty;
        while (tries >= 0 || taskResult == "Done")
        {
             taskResult = await _db.StringGetAsync(taskId);
             tries--;
             await Task.Delay(400);
        }
        
        return taskResult != "Doesnt-Exist";
    }
    
    public async Task<string> InsertTaskIntoQueueAsync(int id,TaskType taskName)
    {
        string taskId = Guid.NewGuid().ToString();
        TaskRedis currTask = null;
        switch (taskName)
        {
            case TaskType.GetSchoolFromId:
            case TaskType.DeleteSchool:
                currTask = new TaskRedis()
                {
                    TaskId = taskId,
                    TaskName = taskName,
                    SchoolId = id
                };
                break;
            case TaskType.GetSchoolByDistrict:
                currTask = new TaskRedis()
                {
                    TaskId = taskId,
                    TaskName = taskName,
                    DistrictId = id
                };
                break;
        }
       
        await _db.ListRightPushAsync("TasksQueue", JsonSerializer.Serialize(currTask));
        return taskId;
    }
    public async Task<string> InsertTaskIntoQueueAsync(string schoolName)
    {
        string taskId = Guid.NewGuid().ToString();
        TaskRedis currTask = new TaskRedis()
        {
            TaskId = taskId,
            TaskName = TaskType.GetSchoolByName,
            SchoolName = schoolName
        };
        await _db.ListRightPushAsync("TasksQueue", JsonSerializer.Serialize(currTask));
        return taskId;
    }
    public async Task<string> InsertTaskIntoQueueAsync(School taskData,TaskType taskName)
    {
        string taskId = Guid.NewGuid().ToString();
        TaskRedis currTask = new TaskRedis()
        {
            TaskId = taskId,
            TaskName = taskName,
            SchoolData = taskData
        };
        await _db.ListRightPushAsync("TasksQueue", JsonSerializer.Serialize(currTask));
        return taskId;

    } 
    public async Task<string> InsertTaskIntoQueueAsync(TaskType taskName)
    {
        string taskId = Guid.NewGuid().ToString();
        TaskRedis currTask = new TaskRedis()
        {
            TaskId = taskId,
            TaskName = taskName,
        };
        
        await _db.ListRightPushAsync("TasksQueue", JsonSerializer.Serialize(currTask));
        return taskId;
    } 
    public async Task<string> InsertTaskIntoQueueAsync(int schoolId,SchoolUpdateDto schoolUpdate,TaskType taskName)
    {
        string taskId = Guid.NewGuid().ToString();
        TaskRedis currTask = new TaskRedis()
        {
            TaskId = taskId,
            TaskName = taskName,
            SchoolId = schoolId,
            SchoolUpdateData = schoolUpdate
        }; 
        await _db.ListRightPushAsync("TasksQueue", JsonSerializer.Serialize(currTask));
        return taskId;
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