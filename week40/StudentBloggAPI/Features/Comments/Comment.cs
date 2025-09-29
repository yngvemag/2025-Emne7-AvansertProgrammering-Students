using StudentBloggAPI.Features.Posts;
using StudentBloggAPI.Features.Users;

namespace StudentBloggAPI.Features.Comments;

public class Comment
{
    public Guid Id { get; set; }
    public Guid PostId { get; set; }
    public Guid UserId { get; set; }
    public string Content { get; set; } = string.Empty;
    public DateTime DateCommented { get; set; }
    
    // Navigation properties
    public virtual Post? Post { get; set; }
    public virtual User? User { get; set; }
}