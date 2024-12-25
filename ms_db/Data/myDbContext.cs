using CommonClasses.Models;
using Microsoft.EntityFrameworkCore;


namespace ms_db.Data;

public class myDbContext :DbContext
{
    public DbSet<School> Schools { get; set; }
    
    public myDbContext(DbContextOptions options) : base(options)
    {
      
    }
    
}