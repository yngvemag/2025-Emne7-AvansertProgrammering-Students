using StudentBloggAPI.Features.Comments;
using StudentBloggAPI.Features.Users;

namespace StudentBloggAPI.Features.Posts;

public class Post
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public DateTime DatePosted { get; set; }
    
    // Navigation property
    public virtual User? User { get; set; }
    public virtual ICollection<Comment> Comments { get; set; } = new HashSet<Comment>();
    
}