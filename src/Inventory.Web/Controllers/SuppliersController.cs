using Inventory.Shared.Dtos;
using Inventory.Web.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Inventory.Web.Controllers;

[Authorize]
public class SuppliersController : Controller
{
    private readonly IInventoryApiClient _api;

    public SuppliersController(IInventoryApiClient api) => _api = api;

    public async Task<IActionResult> Index(string? search)
    {
        ViewBag.Search = search;
        var list = await _api.GetSuppliersAsync(search);
        return View(list);
    }

    public async Task<IActionResult> Details(int id)
    {
        var dto = await _api.GetSupplierAsync(id);
        if (dto is null) return NotFound();
        return View(dto);
    }

    [Authorize(Roles = IdentitySeeder.AdminRole)]
    public IActionResult Create() => View(new SupplierDto());

    [HttpPost, ValidateAntiForgeryToken]
    [Authorize(Roles = IdentitySeeder.AdminRole)]
    public async Task<IActionResult> Create(SupplierDto dto)
    {
        if (!ModelState.IsValid) return View(dto);
        var (ok, error) = await _api.CreateSupplierAsync(dto);
        if (!ok)
        {
            ModelState.AddModelError(string.Empty, error ?? "Failed to create supplier.");
            return View(dto);
        }
        TempData["Success"] = $"Supplier '{dto.Name}' created.";
        return RedirectToAction(nameof(Index));
    }

    [Authorize(Roles = IdentitySeeder.AdminRole)]
    public async Task<IActionResult> Edit(int id)
    {
        var dto = await _api.GetSupplierAsync(id);
        if (dto is null) return NotFound();
        return View(dto);
    }

    [HttpPost, ValidateAntiForgeryToken]
    [Authorize(Roles = IdentitySeeder.AdminRole)]
    public async Task<IActionResult> Edit(int id, SupplierDto dto)
    {
        if (id != dto.Id) return BadRequest();
        if (!ModelState.IsValid) return View(dto);
        var (ok, error) = await _api.UpdateSupplierAsync(dto);
        if (!ok)
        {
            ModelState.AddModelError(string.Empty, error ?? "Failed to update supplier.");
            return View(dto);
        }
        TempData["Success"] = $"Supplier '{dto.Name}' updated.";
        return RedirectToAction(nameof(Index));
    }

    [Authorize(Roles = IdentitySeeder.AdminRole)]
    public async Task<IActionResult> Delete(int id)
    {
        var dto = await _api.GetSupplierAsync(id);
        if (dto is null) return NotFound();
        return View(dto);
    }

    [HttpPost, ActionName("Delete"), ValidateAntiForgeryToken]
    [Authorize(Roles = IdentitySeeder.AdminRole)]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var (ok, error) = await _api.DeleteSupplierAsync(id);
        if (!ok)
        {
            TempData["Error"] = error ?? "Failed to delete supplier.";
            return RedirectToAction(nameof(Delete), new { id });
        }
        TempData["Success"] = "Supplier deleted.";
        return RedirectToAction(nameof(Index));
    }
}
