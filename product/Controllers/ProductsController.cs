using Microsoft.AspNetCore.Mvc;
using ProductApi.DTOs;
using ProductApi.Services;

namespace ProductApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProductsController : ControllerBase
{
    private readonly IProductService _service;

    public ProductsController(IProductService service) => _service = service;

    // GET api/products?category=Electronics&maxPrice=10000&sortBy=price
    [HttpGet]
    public ActionResult<IEnumerable<ProductDto>> GetAll(
        [FromQuery] string? category, [FromQuery] decimal? maxPrice, [FromQuery] string? sortBy)
        => Ok(_service.GetAll(category, maxPrice, sortBy));                     // 200

    // GET api/products/summary
    [HttpGet("summary")]
    public ActionResult<IEnumerable<CategorySummaryDto>> GetSummary()
        => Ok(_service.GetCategorySummary());                                   // 200

    // GET api/products/5
    [HttpGet("{id:int}")]
    public ActionResult<ProductDto> GetById(int id)
    {
        var product = _service.GetById(id);
        if (product is null) return NotFound();                                 // 404
        return Ok(product);                                                     // 200
    }

    // POST api/products
    [HttpPost]
    public ActionResult<ProductDto> Create(CreateProductDto dto)
    {
        var created = _service.Create(dto);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created); // 201
    }

    // PUT api/products/5
    [HttpPut("{id:int}")]
    public IActionResult Update(int id, UpdateProductDto dto)
        => _service.Update(id, dto) ? NoContent() : NotFound();                 // 204 / 404

    // DELETE api/products/5
    [HttpDelete("{id:int}")]
    public IActionResult Delete(int id)
        => _service.Delete(id) ? NoContent() : NotFound();                      // 204 / 404
}