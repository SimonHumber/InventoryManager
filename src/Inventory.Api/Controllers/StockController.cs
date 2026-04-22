using Inventory.Api.Data;
using Inventory.Api.Domain;
using Inventory.Shared.Dtos;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Inventory.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class StockController : ControllerBase
{
    private readonly InventoryDbContext _db;

    public StockController(InventoryDbContext db) => _db = db;

    private static StockTransactionDto ToDto(StockTransaction t) => new()
    {
        Id = t.Id,
        ProductId = t.ProductId,
        ProductName = t.Product?.Name,
        ProductSku = t.Product?.Sku,
        MovementType = t.MovementType,
        Quantity = t.Quantity,
        Note = t.Note,
        PerformedBy = t.PerformedBy,
        OccurredAt = t.OccurredAt
    };

    [HttpGet]
    public async Task<ActionResult<IEnumerable<StockTransactionDto>>> GetAll(
        [FromQuery] int? productId,
        [FromQuery] StockMovementType? type,
        [FromQuery] int take = 100)
    {
        var query = _db.StockTransactions.Include(t => t.Product).AsQueryable();
        if (productId.HasValue) query = query.Where(t => t.ProductId == productId.Value);
        if (type.HasValue) query = query.Where(t => t.MovementType == type.Value);
        var list = await query
            .OrderByDescending(t => t.OccurredAt)
            .Take(Math.Clamp(take, 1, 500))
            .Select(t => ToDto(t))
            .ToListAsync();
        return Ok(list);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<StockTransactionDto>> Get(int id)
    {
        var t = await _db.StockTransactions.Include(x => x.Product).FirstOrDefaultAsync(x => x.Id == id);
        if (t is null) return NotFound();
        return ToDto(t);
    }

    [HttpPost]
    public async Task<ActionResult<StockTransactionDto>> Create([FromBody] StockTransactionDto input)
    {
        if (!ModelState.IsValid) return ValidationProblem(ModelState);
        var product = await _db.Products.FindAsync(input.ProductId);
        if (product is null) return BadRequest(new { message = "Invalid ProductId." });

        var entity = new StockTransaction
        {
            ProductId = input.ProductId,
            MovementType = input.MovementType,
            Quantity = input.Quantity,
            Note = input.Note?.Trim(),
            PerformedBy = input.PerformedBy?.Trim(),
            OccurredAt = DateTime.UtcNow
        };

        switch (input.MovementType)
        {
            case StockMovementType.Inbound:
                product.QuantityInStock += input.Quantity;
                break;
            case StockMovementType.Outbound:
                if (product.QuantityInStock < input.Quantity)
                    return BadRequest(new { message = "Insufficient stock for outbound movement." });
                product.QuantityInStock -= input.Quantity;
                break;
            case StockMovementType.Adjustment:
                product.QuantityInStock = input.Quantity;
                break;
        }

        _db.StockTransactions.Add(entity);
        await _db.SaveChangesAsync();

        await _db.Entry(entity).Reference(e => e.Product).LoadAsync();
        return CreatedAtAction(nameof(Get), new { id = entity.Id }, ToDto(entity));
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var entity = await _db.StockTransactions.FindAsync(id);
        if (entity is null) return NotFound();
        _db.StockTransactions.Remove(entity);
        await _db.SaveChangesAsync();
        return NoContent();
    }
}
