using Microsoft.EntityFrameworkCore;
using StudentBloggAPI.Features.Comments;
using StudentBloggAPI.Features.Posts;
using StudentBloggAPI.Features.Users;

namespace StudentBloggAPI.Data;

public class StudentBloggDbContext(DbContextOptions<StudentBloggDbContext> options) 
    : DbContext(options)
{
    public DbSet<User> Users { get; set; }
    public DbSet<Post> Posts { get; set; }
    public DbSet<Comment> Comments { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<User>(user =>
        {
            user.ToTable("user");
            user.HasKey(u => u.Id);
            // user.Property(u => u.Id).ValueGeneratedOnAdd();
            user.Property(u => u.FirstName).IsRequired().HasMaxLength(100);
            user.Property(u => u.LastName).IsRequired().HasMaxLength(100);
            user.Property(u => u.UserName).IsRequired().HasMaxLength(50);
            //user.HasIndex(u => u.UserName).IsUnique();
            user.Property(u => u.Email).IsRequired().HasMaxLength(100);
            //user.HasIndex(u => u.Email).IsUnique();
            user.Property(u => u.HashedPassword).IsRequired();
            user.Property(u => u.CreatedAt).IsRequired();
            user.Property(u => u.UpdatedAt).IsRequired();
            user.Property(u => u.IsAdminUser).IsRequired().HasDefaultValue(false);
            
            user.HasMany(u => u.Posts)
                .WithOne(p => p.User)
                .HasForeignKey(p => p.UserId);
            
            user.HasMany(u => u.Comments)
                .WithOne( c => c.User)
                .HasForeignKey(c => c.UserId);

        });
        
        modelBuilder.Entity<Post>(post =>
        {
            post.ToTable("post");
            post.HasKey(p => p.Id);
            // post.Property(p => p.Id).ValueGeneratedOnAdd();
            post.Property(p => p.UserId).IsRequired();
            post.Property(p => p.Title).IsRequired().HasMaxLength(50);
            post.Property(p => p.Content).IsRequired();
            post.Property(p => p.DatePosted).IsRequired();
            
            post.HasOne(p => p.User)
                .WithMany(u => u.Posts)
                .HasForeignKey(p => p.UserId);
            
            post.HasMany(p => p.Comments)
                .WithOne(c => c.Post)
                .HasForeignKey(c => c.PostId);
        });

        modelBuilder.Entity<Comment>(comment =>
        {
            comment.ToTable("comment");
            comment.HasKey(c => c.Id);
            comment.Property(c => c.UserId).IsRequired();
            comment.Property(c => c.PostId).IsRequired();
            comment.Property(c => c.Content).IsRequired();
            comment.Property(c => c.DateCommented).IsRequired();
            
            comment.HasOne(c => c.User)
                .WithMany(u => u.Comments)
                .HasForeignKey(c => c.UserId);
            
            comment.HasOne(c => c.Post)
                .WithMany(p => p.Comments)
                .HasForeignKey(c => c.PostId);
        });


    }
}