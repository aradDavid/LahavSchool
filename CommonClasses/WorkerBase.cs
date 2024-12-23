using StackExchange.Redis;

namespace CommonClasses;

public class WorkerBase
{
    protected readonly IConnectionMultiplexer _redis;
    protected readonly IDatabase _db;

    public WorkerBase()
    {
        _redis = ConnectionMultiplexer.Connect("localhost:6379");
        _db = _redis.GetDatabase();
    }
}