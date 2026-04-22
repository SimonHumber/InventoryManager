using Inventory.Api.Data;
using Inventory.Shared.Dtos;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Inventory.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class DashboardController : ControllerBase
{
    private readonly InventoryDbContext _db;

    public DashboardController(InventoryDbContext db) => _db = db;

    [HttpGet]
    public async Task<ActionResult<DashboardDto>> Get()
    {
        var products = await _db.Products.Include(p => p.Category).Include(p => p.Supplier).AsNoTracking().ToListAsync();
        var lowStock = products.Where(p => p.QuantityInStock <= p.ReorderLevel).OrderBy(p => p.QuantityInStock).ToList();

        var recentTx = await _db.StockTransactions
            .Include(t => t.Product)
            .OrderByDescending(t => t.OccurredAt)
            .Take(10)
            .AsNoTracking()
            .ToListAsync();

        var dto = new DashboardDto
        {
            TotalProducts = products.Count,
            TotalCategories = await _db.Categories.CountAsync(),
            TotalSuppliers = await _db.Suppliers.CountAsync(),
            TotalStockUnits = products.Sum(p => p.QuantityInStock),
            LowStockCount = lowStock.Count,
            InventoryValue = products.Sum(p => p.UnitPrice * p.QuantityInStock),
            LowStockProducts = lowStock.Take(10).Select(p => new ProductDto
            {
                Id = p.Id,
                Sku = p.Sku,
                Name = p.Name,
                Description = p.Description,
                UnitPrice = p.UnitPrice,
                QuantityInStock = p.QuantityInStock,
                ReorderLevel = p.ReorderLevel,
                CategoryId = p.CategoryId,
                CategoryName = p.Category?.Name,
                SupplierId = p.SupplierId,
                SupplierName = p.Supplier?.Name,
                CreatedAt = p.CreatedAt
            }).ToList(),
            RecentTransactions = recentTx.Select(t => new StockTransactionDto
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
            }).ToList()
        };

        return dto;
    }
}
