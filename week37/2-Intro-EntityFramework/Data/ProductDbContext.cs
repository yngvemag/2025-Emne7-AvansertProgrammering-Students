using Intro_EntityFramework.Models;
using Microsoft.EntityFrameworkCore;

namespace Intro_EntityFramework.Data;

public class ProductDbContext : DbContext
{
    public ProductDbContext(DbContextOptions<ProductDbContext> options) 
        : base(options)
    {
    }
    
    public DbSet<Product> Products { get; set; }
}