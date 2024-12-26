namespace CommonClasses.Models;

public class WebhookReq
{
    public required string TargetUrl { get; set; }
    public required string ActionType { get; set; }
    public required string ActionResult { get; set; }
    
    
}