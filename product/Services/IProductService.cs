using ProductApi.DTOs;

namespace ProductApi.Services;

public interface IProductService
{
    IEnumerable<ProductDto> GetAll(string? category, decimal? maxPrice, string? sortBy);
    IEnumerable<CategorySummaryDto> GetCategorySummary();
    ProductDto? GetById(int id);
    ProductDto Create(CreateProductDto dto);
    bool Update(int id, UpdateProductDto dto);
    bool Delete(int id);
}