using MinimalApiDemo.DTOs;

namespace MinimalApiDemo.Validation;

public static class ProductValidator
{
    public static Dictionary<string, string[]> Validate(ProductRequest request)
    {
        var errors = new Dictionary<string, string[]>();

        var name = request.Name?.Trim() ?? string.Empty;
        if (name.Length < 2 || name.Length > 100)
            errors["name"] = new[] { "Name must be between 2 and 100 characters." };

        if (request.Price < 1 || request.Price > 1_000_000)
            errors["price"] = new[] { "Price must be between 1 and 1,000,000." };

        if (request.Stock < 0 || request.Stock > 100_000)
            errors["stock"] = new[] { "Stock must be between 0 and 100,000." };

        return errors;
    }
}