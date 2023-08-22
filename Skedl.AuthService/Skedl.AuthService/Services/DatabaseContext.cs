using Microsoft.EntityFrameworkCore;
using Skedl.AuthService.Models;

namespace Skedl.AuthService.Services;

public class DatabaseContext : DbContext
{
    public DbSet<UserCode> UserCodes { get; set; }
    public DbSet<User> Users { get; set; }

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
            "host=localhost;port=5432;database=SkedlAuth;username=postgres;password=123");
    }
}