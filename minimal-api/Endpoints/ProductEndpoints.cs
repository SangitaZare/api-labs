
using MinimalApiDemo.Data;
using MinimalApiDemo.DTOs;
using MinimalApiDemo.Filters;
using MinimalApiDemo.Models;
using MinimalApiDemo.Validation;

namespace MinimalApiDemo.Endpoints;

public static class ProductEndpoints
{
    public static void MapProductEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/products")
                       .WithTags("Products");

        // GET: Retrieve all products
        group.MapGet("/", (ProductStore store) =>
        {
            var products = store.GetAll()
                .Select(p => ToDto(p))
                .ToList();

            return Results.Ok(products);
        });

        // GET: Retrieve one product by ID
        group.MapGet("/{id:int}", (int id, ProductStore store) =>
        {
            var product = store.Get(id);

            return product is null
                ? Results.NotFound()
                : Results.Ok(ToDto(product));
        });

        // POST: Create a new product
        group.MapPost("/", (ProductRequest request, ProductStore store) =>
        {
            var errors = ProductValidator.Validate(request);

            if (errors.Count > 0)
                return Results.ValidationProblem(errors);

            var product = store.Add(
                request.Name.Trim(),
                request.Price,
                request.Stock);

            return Results.Created(
                $"/api/products/{product.Id}",
                ToDto(product));
        });

        // PUT: Update an existing product
        group.MapPut("/{id:int}", (int id, ProductRequest request,
            ProductStore store) =>
        {
            var errors = ProductValidator.Validate(request);

            if (errors.Count > 0)
                return Results.ValidationProblem(errors);

            if (!store.Update(id, request))
                return Results.NotFound();

            return Results.NoContent();
        });

        // DELETE: Delete a product (requires an API key)
        group.MapDelete("/{id:int}", (int id, ProductStore store) =>
        {
            if (!store.Remove(id))
                return Results.NotFound();

            return Results.NoContent();
        })
        .AddEndpointFilter<ApiKeyFilter>();
    }

    private static ProductDto ToDto(Product product)
    {
        return new ProductDto(
            product.Id,
            product.Name,
            product.Price,
            product.Stock);
    }
}
