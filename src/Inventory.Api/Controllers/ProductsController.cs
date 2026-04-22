using Inventory.Api.Data;
using Inventory.Api.Domain;
using Inventory.Shared.Dtos;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Inventory.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProductsController : ControllerBase
{
    private readonly InventoryDbContext _db;

    public ProductsController(InventoryDbContext db) => _db = db;

    private static ProductDto ToDto(Product p) => new()
    {
        Id = p.Id,
        Sku = p.Sku,
        Name = p.Name,
        Description = p.Description,
        UnitPrice = p.UnitPrice,
        QuantityInStock = p.QuantityInStock,
        ReorderLevel = p.ReorderLevel,
        CategoryId = p.CategoryId,
        CategoryName = p.Category != null ? p.Category.Name : null,
        SupplierId = p.SupplierId,
        SupplierName = p.Supplier != null ? p.Supplier.Name : null,
        CreatedAt = p.CreatedAt
    };

    [HttpGet]
    public async Task<ActionResult<IEnumerable<ProductDto>>> GetAll(
        [FromQuery] string? search,
        [FromQuery] int? categoryId,
        [FromQuery] int? supplierId,
        [FromQuery] bool? lowStock)
    {
        var query = _db.Products.Include(p => p.Category).Include(p => p.Supplier).AsQueryable();

        if (!string.IsNullOrWhiteSpace(search))
        {
            var s = search.Trim().ToLower();
            query = query.Where(p =>
                p.Name.ToLower().Contains(s) ||
                p.Sku.ToLower().Contains(s) ||
                (p.Description != null && p.Description.ToLower().Contains(s)));
        }
        if (categoryId.HasValue) query = query.Where(p => p.CategoryId == categoryId.Value);
        if (supplierId.HasValue) query = query.Where(p => p.SupplierId == supplierId.Value);
        if (lowStock == true) query = query.Where(p => p.QuantityInStock <= p.ReorderLevel);

        var list = await query.OrderBy(p => p.Name).Select(p => ToDto(p)).ToListAsync();
        return Ok(list);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<ProductDto>> Get(int id)
    {
        var p = await _db.Products
            .Include(x => x.Category)
            .Include(x => x.Supplier)
            .FirstOrDefaultAsync(x => x.Id == id);
        if (p is null) return NotFound();
        return ToDto(p);
    }

    [HttpPost]
    public async Task<ActionResult<ProductDto>> Create([FromBody] ProductDto input)
    {
        if (!ModelState.IsValid) return ValidationProblem(ModelState);

        if (!await _db.Categories.AnyAsync(c => c.Id == input.CategoryId))
            return BadRequest(new { message = "Invalid CategoryId." });
        if (!await _db.Suppliers.AnyAsync(s => s.Id == input.SupplierId))
            return BadRequest(new { message = "Invalid SupplierId." });

        var entity = new Product
        {
            Sku = input.Sku.Trim(),
            Name = input.Name.Trim(),
            Description = input.Description?.Trim(),
            UnitPrice = input.UnitPrice,
            QuantityInStock = input.QuantityInStock,
            ReorderLevel = input.ReorderLevel,
            CategoryId = input.CategoryId,
            SupplierId = input.SupplierId,
            CreatedAt = DateTime.UtcNow
        };

        _db.Products.Add(entity);
        try
        {
            await _db.SaveChangesAsync();
        }
        catch (DbUpdateException)
        {
            return Conflict(new { message = "A product with this SKU already exists." });
        }

        await _db.Entry(entity).Reference(e => e.Category).LoadAsync();
        await _db.Entry(entity).Reference(e => e.Supplier).LoadAsync();
        var dto = ToDto(entity);
        return CreatedAtAction(nameof(Get), new { id = entity.Id }, dto);
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, [FromBody] ProductDto input)
    {
        if (id != input.Id) return BadRequest();
        if (!ModelState.IsValid) return ValidationProblem(ModelState);

        var entity = await _db.Products.FindAsync(id);
        if (entity is null) return NotFound();

        if (!await _db.Categories.AnyAsync(c => c.Id == input.CategoryId))
            return BadRequest(new { message = "Invalid CategoryId." });
        if (!await _db.Suppliers.AnyAsync(s => s.Id == input.SupplierId))
            return BadRequest(new { message = "Invalid SupplierId." });

        entity.Sku = input.Sku.Trim();
        entity.Name = input.Name.Trim();
        entity.Description = input.Description?.Trim();
        entity.UnitPrice = input.UnitPrice;
        entity.QuantityInStock = input.QuantityInStock;
        entity.ReorderLevel = input.ReorderLevel;
        entity.CategoryId = input.CategoryId;
        entity.SupplierId = input.SupplierId;

        try
        {
            await _db.SaveChangesAsync();
        }
        catch (DbUpdateException)
        {
            return Conflict(new { message = "A product with this SKU already exists." });
        }
        return NoContent();
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var entity = await _db.Products.FindAsync(id);
        if (entity is null) return NotFound();
        _db.Products.Remove(entity);
        await _db.SaveChangesAsync();
        return NoContent();
    }
}
