using Microsoft.EntityFrameworkCore;
using ms_db.Data;
using ms_db.Services;

public class Program
{
    public static async Task Main(string[] args)
    {
        IHost host = Host.CreateDefaultBuilder(args)
            .ConfigureServices((context, services) =>
            {
                string connectionString = "server=127.0.0.1;uid=root;password=;database=myDb";
                services.AddDbContext<myDbContext>(options =>
                    options.UseMySQL(connectionString));
                
                services.AddScoped<WorkerSubServices>();
            })
            .Build();
        
        using (var scope = host.Services.CreateScope())
        {
            var services = scope.ServiceProvider;
            var workerSubServices = services.GetRequiredService<WorkerSubServices>();
            await workerSubServices.PullTasks(); 
        }
        
        await host.RunAsync();
    }
}