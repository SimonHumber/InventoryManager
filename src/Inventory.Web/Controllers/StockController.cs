using Inventory.Shared.Dtos;
using Inventory.Web.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Inventory.Web.Controllers;

[Authorize]
public class StockController : Controller
{
    private readonly IInventoryApiClient _api;

    public StockController(IInventoryApiClient api) => _api = api;

    public async Task<IActionResult> Index(int? productId, StockMovementType? type)
    {
        ViewBag.ProductId = productId;
        ViewBag.Type = type;

        var productsTask = _api.GetProductsAsync();
        var txTask = _api.GetStockTransactionsAsync(productId, type, 200);
        await Task.WhenAll(productsTask, txTask);

        ViewBag.Products = new SelectList(await productsTask, nameof(ProductDto.Id), nameof(ProductDto.Name), productId);
        return View(await txTask);
    }

    public async Task<IActionResult> Create(int? productId)
    {
        await PopulateProducts(productId);
        var dto = new StockTransactionDto
        {
            ProductId = productId ?? 0,
            PerformedBy = User.Identity?.Name,
            Quantity = 1,
            MovementType = StockMovementType.Inbound
        };
        return View(dto);
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(StockTransactionDto dto)
    {
        dto.PerformedBy = User.Identity?.Name;
        if (!ModelState.IsValid)
        {
            await PopulateProducts(dto.ProductId);
            return View(dto);
        }
        var (ok, error) = await _api.CreateStockTransactionAsync(dto);
        if (!ok)
        {
            ModelState.AddModelError(string.Empty, error ?? "Failed to record stock movement.");
            await PopulateProducts(dto.ProductId);
            return View(dto);
        }
        TempData["Success"] = "Stock movement recorded.";
        return RedirectToAction(nameof(Index));
    }

    [Authorize(Roles = IdentitySeeder.AdminRole)]
    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        var (ok, error) = await _api.DeleteStockTransactionAsync(id);
        TempData[ok ? "Success" : "Error"] = ok ? "Stock movement removed." : (error ?? "Failed to delete.");
        return RedirectToAction(nameof(Index));
    }

    private async Task PopulateProducts(int? productId)
    {
        var products = await _api.GetProductsAsync();
        ViewBag.Products = new SelectList(products, nameof(ProductDto.Id), nameof(ProductDto.Name), productId);
    }
}
