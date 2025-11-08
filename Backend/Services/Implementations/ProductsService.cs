using Backend.Models;
using Backend.Models.DTOs;
using Backend.Services.Interfaces;

namespace Backend.Services.Implementations;

public class ProductsService : IProductsService
{
    private static readonly List<Product> _products = new();

    public ProductsService()
    {
        if (!_products.Any())
        {
            // Initialize with some data if empty
            _products.AddRange(new Product[] {
                    new Product { Id = 1, Name = "Laptop Gamer", Description = "Laptop de alta gama para juegos", Price = 1500.00m, ImageUrl = "images/laptop.jpg", Stock = 10, Category = "Electrónica" },
                    new Product { Id = 2, Name = "Smartphone", Description = "Teléfono inteligente de última generación", Price = 800.00m, ImageUrl = "images/smartphone.jpg", Stock = 25, Category = "Electrónica" },
                    new Product { Id = 3, Name = "Auriculares Bluetooth", Description = "Auriculares con cancelación de ruido", Price = 150.00m, ImageUrl = "images/headphones.jpg", Stock = 50, Category = "Accesorios" },
                    new Product { Id = 4, Name = "Teclado Mecánico", Description = "Teclado para gaming y programación", Price = 120.00m, ImageUrl = "images/keyboard.jpg", Stock = 30, Category = "Accesorios" },
                    new Product { Id = 5, Name = "Monitor 4K", Description = "Monitor de 27 pulgadas con resolución 4K", Price = 450.00m, ImageUrl = "images/monitor.jpg", Stock = 15, Category = "Electrónica" },
                });
        }
    }

    public Task<IEnumerable<Product>> GetProducts(string? category, int page, int pageSize)
    {
        var query = _products.AsQueryable();

        if (!string.IsNullOrEmpty(category))
        {
            query = query.Where(p => p.Category == category);
        }

        var paginatedProducts = query.Skip((page - 1) * pageSize).Take(pageSize).ToList();
        return Task.FromResult<IEnumerable<Product>>(paginatedProducts);
    }

    public Task<Product?> GetProductById(int id)
    {
        var product = _products.FirstOrDefault(p => p.Id == id);
        return Task.FromResult(product);
    }

    public Task<Product> CreateProduct(ProductDto productDto)
    {
        var newProduct = new Product
        {
            Id = _products.Any() ? _products.Max(p => p.Id) + 1 : 1,
            Name = productDto.Name,
            Description = productDto.Description,
            Price = productDto.Price,
            ImageUrl = productDto.ImageUrl,
            Stock = productDto.Stock,
            Category = productDto.Category
        };
        _products.Add(newProduct);
        return Task.FromResult(newProduct);
    }

    public Task<Product?> UpdateProduct(int id, ProductDto productDto)
    {
        var product = _products.FirstOrDefault(p => p.Id == id);
        if (product == null) return Task.FromResult<Product?>(null);

        product.Name = productDto.Name;
        product.Description = productDto.Description;
        product.Price = productDto.Price;
        product.ImageUrl = productDto.ImageUrl;
        product.Stock = productDto.Stock;
        product.Category = productDto.Category;

        return Task.FromResult<Product?>(product);
    }

    public Task<bool> DeleteProduct(int id)
    {
        var product = _products.FirstOrDefault(p => p.Id == id);
        if (product == null) return Task.FromResult(false);

        _products.Remove(product);
        return Task.FromResult(true);
    }
}