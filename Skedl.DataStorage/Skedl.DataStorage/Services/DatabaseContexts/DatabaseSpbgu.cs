using Microsoft.EntityFrameworkCore;
using Skedl.DataStorage.Models;

namespace Skedl.DataStorage.Services.DatabaseContexts;

public class DatabaseSpbgu : DbContext
{
    public DbSet<Group> Groups { get; set; }

    public DatabaseSpbgu()
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
            "host=localhost;port=5432;database=SkedlDataSpbgu;username=postgres;password=123");
    }
}