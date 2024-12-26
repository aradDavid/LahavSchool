

using ms_webhooks.Services;

class Program
{
    public static async Task Main(string[] args)
    {
        IHost host = Host.CreateDefaultBuilder(args)
            .ConfigureServices((context, services) =>
            {
                services.AddScoped<WebhooksWorker>();
            })
            .Build();
        
        using (var scope = host.Services.CreateScope())
        {
            var services = scope.ServiceProvider;
            var workerSubServices = services.GetRequiredService<WebhooksWorker>();
            await workerSubServices.HandleTasks(); 
        }
        
        await host.RunAsync();
    }
}

