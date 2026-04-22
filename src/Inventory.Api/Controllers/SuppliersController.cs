using Inventory.Api.Data;
using Inventory.Api.Domain;
using Inventory.Shared.Dtos;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Inventory.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SuppliersController : ControllerBase
{
    private readonly InventoryDbContext _db;

    public SuppliersController(InventoryDbContext db) => _db = db;

    private static SupplierDto ToDto(Supplier s) => new()
    {
        Id = s.Id,
        Name = s.Name,
        ContactName = s.ContactName,
        Email = s.Email,
        Phone = s.Phone,
        Address = s.Address
    };

    [HttpGet]
    public async Task<ActionResult<IEnumerable<SupplierDto>>> GetAll([FromQuery] string? search)
    {
        var query = _db.Suppliers.AsQueryable();
        if (!string.IsNullOrWhiteSpace(search))
        {
            var s = search.Trim().ToLower();
            query = query.Where(x =>
                x.Name.ToLower().Contains(s) ||
                (x.ContactName != null && x.ContactName.ToLower().Contains(s)) ||
                (x.Email != null && x.Email.ToLower().Contains(s)));
        }
        var list = await query.OrderBy(x => x.Name).Select(x => ToDto(x)).ToListAsync();
        return Ok(list);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<SupplierDto>> Get(int id)
    {
        var s = await _db.Suppliers.FindAsync(id);
        if (s is null) return NotFound();
        return ToDto(s);
    }

    [HttpPost]
    public async Task<ActionResult<SupplierDto>> Create([FromBody] SupplierDto input)
    {
        if (!ModelState.IsValid) return ValidationProblem(ModelState);
        var entity = new Supplier
        {
            Name = input.Name.Trim(),
            ContactName = input.ContactName?.Trim(),
            Email = input.Email?.Trim(),
            Phone = input.Phone?.Trim(),
            Address = input.Address?.Trim()
        };
        _db.Suppliers.Add(entity);
        await _db.SaveChangesAsync();
        input.Id = entity.Id;
        return CreatedAtAction(nameof(Get), new { id = entity.Id }, input);
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, [FromBody] SupplierDto input)
    {
        if (id != input.Id) return BadRequest();
        if (!ModelState.IsValid) return ValidationProblem(ModelState);
        var entity = await _db.Suppliers.FindAsync(id);
        if (entity is null) return NotFound();
        entity.Name = input.Name.Trim();
        entity.ContactName = input.ContactName?.Trim();
        entity.Email = input.Email?.Trim();
        entity.Phone = input.Phone?.Trim();
        entity.Address = input.Address?.Trim();
        await _db.SaveChangesAsync();
        return NoContent();
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var entity = await _db.Suppliers.Include(s => s.Products).FirstOrDefaultAsync(s => s.Id == id);
        if (entity is null) return NotFound();
        if (entity.Products.Any())
            return Conflict(new { message = "Cannot delete a supplier that has products." });
        _db.Suppliers.Remove(entity);
        await _db.SaveChangesAsync();
        return NoContent();
    }
}
