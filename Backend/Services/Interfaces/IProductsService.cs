using Backend.Models;
using Backend.Models.DTOs;

namespace Backend.Services.Interfaces;

public interface IProductsService
{
    Task<IEnumerable<Product>> GetProducts(string? category, int page, int pageSize);
    Task<Product?> GetProductById(int id);
    Task<Product> CreateProduct(ProductDto productDto);
    Task<Product?> UpdateProduct(int id, ProductDto productDto);
    Task<bool> DeleteProduct(int id);
}