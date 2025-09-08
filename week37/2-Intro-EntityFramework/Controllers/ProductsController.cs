
using Intro_EntityFramework.Data;
using Intro_EntityFramework.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Intro_EntityFramework.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public class ProductsController : ControllerBase
{
    private readonly ProductDbContext _productDbContext;
    public ProductsController(ProductDbContext context)
    {
        _productDbContext = context;
    }
    
    // IResult
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Product>>> GetProductsAsync()
    {
        return await _productDbContext.Products.ToListAsync();
    }
    
    [HttpGet("{id}")]
    public async Task<ActionResult<Product>> GetProductAsync(int id)
    {
        var product = await _productDbContext.Products.FindAsync(id);
        
        return product is null 
            ? NotFound() 
            : Ok(product);
        
    }
    
    [HttpPost]
    public async Task<ActionResult> AddProductAsync(Product product)
    {
        _productDbContext.Products.Add(product);
        await _productDbContext.SaveChangesAsync();
        return CreatedAtAction("AddProduct", new { id = product.Id }, product);
        // return Ok("Product added successfully");
    }

    [HttpPut("{id}")]
    public async Task<ActionResult> UpdateProductAsync(int id, Product product)
    {
        var existingProduct = await _productDbContext.Products.FindAsync(id);
        if (existingProduct is null)
        {
            return NotFound();
        }
        
        // update properties
        existingProduct.Name = string.IsNullOrEmpty(product.Name) 
            ? existingProduct.Name 
            : product.Name;
        existingProduct.Price = product.Price == 0 
            ? existingProduct.Price
            : product.Price;
        existingProduct.Stock = product.Stock == 0
            ? existingProduct.Stock
            : product.Stock;
        
        await _productDbContext.SaveChangesAsync();
        return Ok("Product updated successfully");
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> DeleteProductAsync(int id)
    {
        var product = await _productDbContext.Products.FindAsync(id);
        if (product is null) return NotFound();
        
        _productDbContext.Products.Remove(product);
        await _productDbContext.SaveChangesAsync();
        return Ok("Product deleted successfully");
    }
}