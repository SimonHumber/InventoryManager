using Inventory.Shared.Dtos;
using Inventory.Web.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Inventory.Web.Controllers;

[Authorize]
public class ProductsController : Controller
{
    private readonly IInventoryApiClient _api;

    public ProductsController(IInventoryApiClient api) => _api = api;

    public async Task<IActionResult> Index(string? search, int? categoryId, int? supplierId, bool? lowStock)
    {
        ViewBag.Search = search;
        ViewBag.CategoryId = categoryId;
        ViewBag.SupplierId = supplierId;
        ViewBag.LowStock = lowStock;

        var categoriesTask = _api.GetCategoriesAsync();
        var suppliersTask = _api.GetSuppliersAsync();
        var productsTask = _api.GetProductsAsync(search, categoryId, supplierId, lowStock);
        await Task.WhenAll(categoriesTask, suppliersTask, productsTask);

        ViewBag.Categories = new SelectList(await categoriesTask, nameof(CategoryDto.Id), nameof(CategoryDto.Name), categoryId);
        ViewBag.Suppliers = new SelectList(await suppliersTask, nameof(SupplierDto.Id), nameof(SupplierDto.Name), supplierId);
        return View(await productsTask);
    }

    public async Task<IActionResult> Details(int id)
    {
        var dto = await _api.GetProductAsync(id);
        if (dto is null) return NotFound();
        return View(dto);
    }

    [Authorize(Roles = IdentitySeeder.AdminRole)]
    public async Task<IActionResult> Create()
    {
        await PopulateLookups();
        return View(new ProductDto());
    }

    [HttpPost, ValidateAntiForgeryToken]
    [Authorize(Roles = IdentitySeeder.AdminRole)]
    public async Task<IActionResult> Create(ProductDto dto)
    {
        if (!ModelState.IsValid)
        {
            await PopulateLookups(dto.CategoryId, dto.SupplierId);
            return View(dto);
        }
        var (ok, error) = await _api.CreateProductAsync(dto);
        if (!ok)
        {
            ModelState.AddModelError(string.Empty, error ?? "Failed to create product.");
            await PopulateLookups(dto.CategoryId, dto.SupplierId);
            return View(dto);
        }
        TempData["Success"] = $"Product '{dto.Name}' created.";
        return RedirectToAction(nameof(Index));
    }

    [Authorize(Roles = IdentitySeeder.AdminRole)]
    public async Task<IActionResult> Edit(int id)
    {
        var dto = await _api.GetProductAsync(id);
        if (dto is null) return NotFound();
        await PopulateLookups(dto.CategoryId, dto.SupplierId);
        return View(dto);
    }

    [HttpPost, ValidateAntiForgeryToken]
    [Authorize(Roles = IdentitySeeder.AdminRole)]
    public async Task<IActionResult> Edit(int id, ProductDto dto)
    {
        if (id != dto.Id) return BadRequest();
        if (!ModelState.IsValid)
        {
            await PopulateLookups(dto.CategoryId, dto.SupplierId);
            return View(dto);
        }
        var (ok, error) = await _api.UpdateProductAsync(dto);
        if (!ok)
        {
            ModelState.AddModelError(string.Empty, error ?? "Failed to update product.");
            await PopulateLookups(dto.CategoryId, dto.SupplierId);
            return View(dto);
        }
        TempData["Success"] = $"Product '{dto.Name}' updated.";
        return RedirectToAction(nameof(Index));
    }

    [Authorize(Roles = IdentitySeeder.AdminRole)]
    public async Task<IActionResult> Delete(int id)
    {
        var dto = await _api.GetProductAsync(id);
        if (dto is null) return NotFound();
        return View(dto);
    }

    [HttpPost, ActionName("Delete"), ValidateAntiForgeryToken]
    [Authorize(Roles = IdentitySeeder.AdminRole)]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var (ok, error) = await _api.DeleteProductAsync(id);
        if (!ok)
        {
            TempData["Error"] = error ?? "Failed to delete product.";
            return RedirectToAction(nameof(Delete), new { id });
        }
        TempData["Success"] = "Product deleted.";
        return RedirectToAction(nameof(Index));
    }

    private async Task PopulateLookups(int? categoryId = null, int? supplierId = null)
    {
        var categories = await _api.GetCategoriesAsync();
        var suppliers = await _api.GetSuppliersAsync();
        ViewBag.Categories = new SelectList(categories, nameof(CategoryDto.Id), nameof(CategoryDto.Name), categoryId);
        ViewBag.Suppliers = new SelectList(suppliers, nameof(SupplierDto.Id), nameof(SupplierDto.Name), supplierId);
    }
}
