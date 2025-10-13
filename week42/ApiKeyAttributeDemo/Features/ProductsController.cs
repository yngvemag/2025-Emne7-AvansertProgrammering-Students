using ApiKeyAttributeDemo.Security.Attributes;
using Microsoft.AspNetCore.Mvc;

namespace ApiKeyAttributeDemo.Features;

[ApiController]
[Route("api/v1/[controller]")]
public class ProductsController : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetProductsAsync()
    {
        await Task.Delay(10);
        var products = new List<ProductDto>
        {
            new ProductDto { Id = 1, Name = "Product 1", Price = 10.0m, Stock = 100 },
            new ProductDto { Id = 2, Name = "Product 2", Price = 20.0m, Stock = 200 },
            new ProductDto { Id = 3, Name = "Product 3", Price = 30.0m, Stock = 300 }
        };

        return Ok(products);
    }

    [ApiKey]
    [HttpGet("{id}")]
    public async Task<IActionResult> GetProductByIdAsync(int id)
    {
        await Task.Delay(10);
        var product = new ProductDto 
            { Id = id, 
                Name = $"Product {id}", 
                Price = 10.0m * id, 
                Stock = 100 * id 
            };
        return Ok(product);
    }
}