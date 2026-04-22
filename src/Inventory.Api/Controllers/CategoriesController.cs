using Inventory.Api.Data;
using Inventory.Api.Domain;
using Inventory.Shared.Dtos;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Inventory.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CategoriesController : ControllerBase
{
    private readonly InventoryDbContext _db;

    public CategoriesController(InventoryDbContext db) => _db = db;

    [HttpGet]
    public async Task<ActionResult<IEnumerable<CategoryDto>>> GetAll([FromQuery] string? search)
    {
        var query = _db.Categories.Include(c => c.Products).AsQueryable();
        if (!string.IsNullOrWhiteSpace(search))
        {
            var s = search.Trim().ToLower();
            query = query.Where(c =>
                c.Name.ToLower().Contains(s) ||
                (c.Description != null && c.Description.ToLower().Contains(s)));
        }

        var list = await query
            .OrderBy(c => c.Name)
            .Select(c => new CategoryDto
            {
                Id = c.Id,
                Name = c.Name,
                Description = c.Description,
                ProductCount = c.Products.Count
            })
            .ToListAsync();

        return Ok(list);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<CategoryDto>> Get(int id)
    {
        var c = await _db.Categories.Include(x => x.Products).FirstOrDefaultAsync(x => x.Id == id);
        if (c is null) return NotFound();
        return new CategoryDto
        {
            Id = c.Id,
            Name = c.Name,
            Description = c.Description,
            ProductCount = c.Products.Count
        };
    }

    [HttpPost]
    public async Task<ActionResult<CategoryDto>> Create([FromBody] CategoryDto input)
    {
        if (!ModelState.IsValid) return ValidationProblem(ModelState);
        var entity = new Category { Name = input.Name.Trim(), Description = input.Description?.Trim() };
        _db.Categories.Add(entity);
        try
        {
            await _db.SaveChangesAsync();
        }
        catch (DbUpdateException)
        {
            return Conflict(new { message = "A category with this name already exists." });
        }
        input.Id = entity.Id;
        return CreatedAtAction(nameof(Get), new { id = entity.Id }, input);
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, [FromBody] CategoryDto input)
    {
        if (id != input.Id) return BadRequest();
        if (!ModelState.IsValid) return ValidationProblem(ModelState);
        var entity = await _db.Categories.FindAsync(id);
        if (entity is null) return NotFound();
        entity.Name = input.Name.Trim();
        entity.Description = input.Description?.Trim();
        try
        {
            await _db.SaveChangesAsync();
        }
        catch (DbUpdateException)
        {
            return Conflict(new { message = "A category with this name already exists." });
        }
        return NoContent();
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var entity = await _db.Categories.Include(c => c.Products).FirstOrDefaultAsync(c => c.Id == id);
        if (entity is null) return NotFound();
        if (entity.Products.Any())
            return Conflict(new { message = "Cannot delete a category that has products." });
        _db.Categories.Remove(entity);
        await _db.SaveChangesAsync();
        return NoContent();
    }
}
