using StudentBloggAPI.Features.Comments;
using StudentBloggAPI.Features.Posts;

namespace StudentBloggAPI.Features.Users;

public readonly record struct UserId(Guid Value)
{
    public static UserId NewId() => new UserId(Guid.NewGuid());
    public static UserId Empty => new UserId(Guid.Empty);
}

public class User
{
    public Guid Id { get; set; }
    //public UserId Id { get; set; } 
    
    public string FirstName { get; set; } = string.Empty;
    
    public string LastName { get; set; } = string.Empty;
    
    public string UserName { get; set; } = string.Empty;
    
    public string Email { get; set; } = string.Empty;
    
    public string HashedPassword { get; set; } = string.Empty;
    
    public DateTime CreatedAt { get; set; }
    
    public DateTime UpdatedAt { get; set; }

    public bool IsAdminUser { get; set; } = false;
    
    // Navigation properties
    public virtual ICollection<Post> Posts { get; set; } = new HashSet<Post>();
    public virtual ICollection<Comment> Comments { get; set; } = new HashSet<Comment>();
}