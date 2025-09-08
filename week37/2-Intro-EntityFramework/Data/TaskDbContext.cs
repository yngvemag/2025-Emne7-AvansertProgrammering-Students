using Intro_EntityFramework.Models;
using Microsoft.EntityFrameworkCore;

namespace Intro_EntityFramework.Data;

public class TaskDbContext : DbContext
{
    public TaskDbContext(DbContextOptions<TaskDbContext> options) 
        : base(options)
    {
        
    }
    
    public DbSet<TaskItem> Tasks { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<TaskItem>( entity =>
        {
           entity.ToTable("tasks");
           entity.HasKey(t => t.Id); // set primary key
           entity.Property( p => p.Id)
               .ValueGeneratedOnAdd();
           
           entity.Property(p => p.Title)
               .IsRequired()
               .HasMaxLength(100);

           entity.Property(p => p.IsCompleted)
               .IsRequired();
           
           entity.Property(p => p.DueDate)
               .HasColumnType("datetime");
        });
    }
}