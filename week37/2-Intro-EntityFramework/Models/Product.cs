using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Intro_EntityFramework.Models;

[Table("products")]
public class Product
{
    [Key]
    public int Id { get; set; }
    
    [Required]
    [StringLength(50)]
    public string Name { get; set; } = string.Empty;
    
    // Decimal(18,2)
    [Column(TypeName = "decimal(18, 2)")]
    public decimal Price { get; set; }
    
    [Required]
    public int Stock { get; set; }
}