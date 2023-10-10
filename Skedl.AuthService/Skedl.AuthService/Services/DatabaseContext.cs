using Microsoft.EntityFrameworkCore;
using Skedl.AuthService.Models;

namespace Skedl.AuthService.Services;

public class DatabaseContext : DbContext
{
    public DbSet<UserCode> UserCodes { get; set; }
    public DbSet<User> Users { get; set; }

    public DatabaseContext(DbContextOptions<DatabaseContext> options) : base(options)
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
}