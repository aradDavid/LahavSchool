using System.Text.Json;
using CommonClasses;
using CommonClasses.Models;

namespace API.Services;

public class WebhookService : WorkerBase
{
    
    public async Task<string> InsertTaskIntoQueueAsync(string targetUrl , string taskName,string taskResult)
    {
        string taskId = Guid.NewGuid().ToString();
        var webhookObj = new WebhookReq()
        {
            TargetUrl = targetUrl,
            ActionType = taskName,
            ActionResult = taskResult
        };
        await _db.ListRightPushAsync("webhooksQueue", JsonSerializer.Serialize(webhookObj));
        return taskId;
    } 
}