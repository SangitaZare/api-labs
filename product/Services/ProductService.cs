using ProductApi.DTOs;
using ProductApi.Models;

namespace ProductApi.Services;

public class ProductService : IProductService
{
    private readonly List<Product> _products = new()
    {
        new Product { Id = 1, Name = "Laptop", Category = "Electronics", Price = 55000, Stock = 10, CreatedAt = DateTime.UtcNow },
        new Product { Id = 2, Name = "Mouse", Category = "Electronics", Price = 600, Stock = 0, CreatedAt = DateTime.UtcNow },
        new Product { Id = 3, Name = "Notebook", Category = "Stationery", Price = 80, Stock = 200, CreatedAt = DateTime.UtcNow },
        new Product { Id = 4, Name = "Office Chair", Category = "Furniture", Price = 7500, Stock = 5, CreatedAt = DateTime.UtcNow }
    };

    // LINQ: Where (filter), OrderBy (sort), Select (project to DTO)
    public IEnumerable<ProductDto> GetAll(string? category, decimal? maxPrice, string? sortBy)
    {
        IEnumerable<Product> query = _products;

        if (!string.IsNullOrWhiteSpace(category))
            query = query.Where(p => p.Category.Equals(category, StringComparison.OrdinalIgnoreCase));

        if (maxPrice.HasValue)
            query = query.Where(p => p.Price <= maxPrice.Value);

        query = sortBy?.ToLower() switch
        {
            "price" => query.OrderBy(p => p.Price),
            "price_desc" => query.OrderByDescending(p => p.Price),
            _ => query.OrderBy(p => p.Name)
        };

        return query.Select(ToDto).ToList();
    }

    // LINQ: GroupBy + aggregates (Count, Average, Sum)
    public IEnumerable<CategorySummaryDto> GetCategorySummary() =>
        _products
            .GroupBy(p => p.Category)
            .Select(g => new CategorySummaryDto(
                g.Key,
                g.Count(),
                g.Average(p => p.Price),
                g.Sum(p => p.Stock)))
            .OrderByDescending(s => s.ProductCount)
            .ToList();

    // LINQ: FirstOrDefault
    public ProductDto? GetById(int id)
    {
        var product = _products.FirstOrDefault(p => p.Id == id);
        return product is null ? null : ToDto(product);
    }

    public ProductDto Create(CreateProductDto dto)
    {
        var product = new Product
        {
            Id = _products.Any() ? _products.Max(p => p.Id) + 1 : 1,   // LINQ: Any, Max
            Name = dto.Name,
            Category = dto.Category,
            Price = dto.Price,
            Stock = dto.Stock,
            CreatedAt = DateTime.UtcNow
        };
        _products.Add(product);
        return ToDto(product);
    }

    public bool Update(int id, UpdateProductDto dto)
    {
        var product = _products.FirstOrDefault(p => p.Id == id);
        if (product is null) return false;

        product.Name = dto.Name;
        product.Category = dto.Category;
        product.Price = dto.Price;
        product.Stock = dto.Stock;
        return true;
    }

    public bool Delete(int id)
    {
        var product = _products.FirstOrDefault(p => p.Id == id);
        if (product is null) return false;

        _products.Remove(product);
        return true;
    }

    // Model → DTO mapping (hides CreatedAt, converts Stock to InStock)
    private static ProductDto ToDto(Product p) =>
        new(p.Id, p.Name, p.Category, p.Price, p.Stock > 0);
}