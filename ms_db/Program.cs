using StackExchange.Redis;
using CommonClasses.Data;
using ms_db.Services;
using Microsoft.EntityFrameworkCore;

public class Program
{
    public static async Task Main(string[] args)
    {
        var dbContextOptions = new DbContextOptionsBuilder<myDbContext>()
            .UseSqlite("Data Source=SchoolsProjectDb.db")  // Configure database connection
            .Options;
        var dbContext = new myDbContext(dbContextOptions);

        var taskConsumer = new WorkerSubServices(dbContext);
    
        
        await taskConsumer.PullTasks(CancellationToken.None);

    }
}
    