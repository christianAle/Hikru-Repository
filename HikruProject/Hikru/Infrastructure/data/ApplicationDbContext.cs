using Microsoft.EntityFrameworkCore;
using HikruCodeChallenge.Domain.Entities;

namespace HikruCodeChallenge.Infrastructure.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    }

    public DbSet<Assessment> Assessments { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Assessment>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).ValueGeneratedOnAdd();
            entity.Property(e => e.Status).HasConversion<int>();
            entity.HasIndex(e => e.Title);
            entity.HasIndex(e => e.Status);
            entity.HasIndex(e => e.RecruiterId);
            entity.HasIndex(e => e.DepartmentId);
        });
    }
}
