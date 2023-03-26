using Microsoft.EntityFrameworkCore;

namespace WebApi.Models;

public class LMSContext : DbContext
{
    public LMSContext(DbContextOptions<LMSContext> options)
        : base(options)
    {
    }

    public DbSet<Assignment> Assignments { get; set; } = null!;
    public DbSet<Course> Courses { get; set; } = null!;
    public DbSet<Module> Modules { get; set; } = null!;
}