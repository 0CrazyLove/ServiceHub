using Backend.Models;
using Backend.DTOs;
using Backend.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
namespace Backend.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProductsController(IProductsService productsService) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Product>>> GetProducts([FromQuery] string? category, [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
    {
        var paginatedProducts = await productsService.GetProducts(category, page, pageSize);
        return Ok(paginatedProducts);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Product>> GetProduct(int id)
    {
        var product = await productsService.GetProductById(id);
        if (product == null)
        {
            return NotFound();
        }
        return Ok(product);
    }
    
    [HttpPost]
    // [Authorize(Roles = "Admin")]
    public async Task<ActionResult<Product>> CreateProduct(ProductDto productDto)
    {
        var newProduct = await productsService.CreateProduct(productDto);
        return CreatedAtAction(nameof(GetProduct), new { id = newProduct.Id }, newProduct);
    }

    [HttpPut("{id}")]
    // [Authorize(Roles = "Admin")]
    public async Task<IActionResult> UpdateProduct(int id, ProductDto productDto)
    {
        var updatedProduct = await productsService.UpdateProduct(id, productDto);
        if (updatedProduct == null)
        {
            return NotFound();
        }
        return NoContent();
    }

    [HttpDelete("{id}")]
    // [Authorize(Roles = "Admin")]
    public async Task<IActionResult> DeleteProduct(int id)
    {
        var result = await productsService.DeleteProduct(id);
        if (!result)
        {
            return NotFound();
        }
        return NoContent();
    }
}
