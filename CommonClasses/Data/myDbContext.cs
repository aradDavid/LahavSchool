using CommonClasses.Models;
using Microsoft.EntityFrameworkCore;

namespace CommonClasses.Data;

public class myDbContext : DbContext
{
    public DbSet<School> Schools { get; set; }
    public DbSet<License> Licenses { get; set; }
    public DbSet<District> Districts { get; set; }
    

    public myDbContext(DbContextOptions i_Options) : base(i_Options)
    {
      
    }
    
}