using Microsoft.EntityFrameworkCore;
using System;

namespace WebApi.Models
{
    public class LMSContext : DbContext
    {
        public LMSContext(DbContextOptions<LMSContext> options)
        : base(options)
    {
    }
    
        public DbSet<Assignment> Assignments { get; set; }
        public DbSet<Course> Courses { get; set; }
        public DbSet<Module> Modules { get; set; }

        // protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        // {
        //     optionsBuilder.UseSqlite("Data Source=lms.db");
        // }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Course>()
                .HasMany(c => c.Modules)
                .WithOne(m => m.Course)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Module>()
                .HasMany(m => m.Assignments)
                .WithOne(a => a.Module)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
