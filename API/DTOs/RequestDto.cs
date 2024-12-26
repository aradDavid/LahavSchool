namespace API.DTOs;

public class RequestDto
{
    public string? webhooks_url { get; set; }
    public CancellationToken CancellationToken = default;
}