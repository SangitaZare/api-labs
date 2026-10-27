using System.ComponentModel.DataAnnotations;

namespace ProductApi.DTOs;

// OUTPUT: what the API returns
public record ProductDto(int Id, string Name, string Category, decimal Price, bool InStock);

// OUTPUT: for the LINQ GroupBy summary endpoint
public record CategorySummaryDto(string Category, int ProductCount, decimal AveragePrice, int TotalStock);

// INPUT: what the client sends to create a product
public class CreateProductDto
{
    [Required, StringLength(100, MinimumLength = 2)]
    public string Name { get; set; } = string.Empty;

    [Required]
    public string Category { get; set; } = string.Empty;

    [Range(0.01, 1000000)]
    public decimal Price { get; set; }

    [Range(0, int.MaxValue)]
    public int Stock { get; set; }
}

// INPUT: what the client sends to update (same fields here)
public class UpdateProductDto : CreateProductDto { }