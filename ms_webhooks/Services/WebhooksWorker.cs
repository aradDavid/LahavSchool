using System.Text;
using System.Text.Json;
using CommonClasses;
using CommonClasses.Models;
using Microsoft.AspNetCore.Mvc;

namespace ms_webhooks.Services;

public class WebhooksWorker : WorkerBase
{
    private readonly ILogger<WebhooksWorker> _logger;

    public WebhooksWorker(ILogger<WebhooksWorker> logger)
    {
        _logger = logger;
    }

    
    private async Task sendAsync(WebhookReq webhookReq)
    {
        var client = new HttpClient();
        string resultBack = $"User has tried to :{webhookReq.ActionType} and was : {webhookReq.ActionResult}";
        var content = new StringContent(JsonSerializer.Serialize(resultBack), Encoding.UTF8, "application/json");
        try
        {
            var response = await client.PostAsync(webhookReq.TargetUrl, content);
            _logger.LogInformation(JsonSerializer.Serialize(response));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message);
        }
        
    }
    public async Task HandleTasks(CancellationToken cancellationToken = default)
    {
        while (!cancellationToken.IsCancellationRequested)
        {
            var webhookTaskJson = await _db.ListLeftPopAsync("webhooksQueue");
            if (!webhookTaskJson.IsNullOrEmpty)
            {
                var webHookData = JsonSerializer.Deserialize<WebhookReq>(webhookTaskJson);
                _logger.LogInformation($"Sending Webhook to {webHookData.TargetUrl}");
                await sendAsync(webHookData);
            }
        }
    }
}