using Microsoft.EntityFrameworkCore;
using Skedl.DataStorage.Models;

namespace Skedl.DataStorage.Services;

public class DatabaseContext : DbContext
{
    public DbSet<Group> Groups { get; set; }

    public DatabaseContext()
    {
        try
        {
            Database.EnsureCreated();
        }
        catch (Exception)
        {
            // ignored
        }
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseNpgsql(
            "host=localhost;port=5432;database=SkedlData;username=postgres;password=123");
    }
}