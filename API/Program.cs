
using CommonClasses.Data;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddDbContext<myDbContext>(options =>
{
    options.UseSqlite(builder.Configuration.GetConnectionString("myDbConnection"));
});

var app = builder.Build();
// Configure the HTTP request pipeline.
app.MapControllers();

app.Run();
