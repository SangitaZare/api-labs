using System.Collections.Concurrent;
using MinimalApiDemo.DTOs;
using MinimalApiDemo.Models;

namespace MinimalApiDemo.Data;

public class ProductStore
{
    private readonly ConcurrentDictionary<int, Product> _products = new();
    private int _lastId;

    public ProductStore()
    {
        Add("iPhone 15", 70000, 10);
        Add("Samsung S24", 65000, 5);
        Add("Dell Laptop", 55000, 8);
        Add("Boat Earbuds", 1500, 50);
    }

    public Product Add(string name, decimal price, int stock)
    {
        var product = new Product
        {
            Id = Interlocked.Increment(ref _lastId),
            Name = name,
            Price = price,
            Stock = stock
        };
        _products[product.Id] = product;
        return product;
    }

    public IEnumerable<Product> GetAll() => _products.Values.OrderBy(p => p.Id);

    public Product? Get(int id) => _products.TryGetValue(id, out var product) ? product : null;

    public bool Update(int id, ProductRequest request)
    {
        if (!_products.TryGetValue(id, out var product)) return false;

        product.Name = request.Name.Trim();
        product.Price = request.Price;
        product.Stock = request.Stock;
        return true;
    }

    public bool Remove(int id) => _products.TryRemove(id, out _);
}