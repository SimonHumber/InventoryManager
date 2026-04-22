using Inventory.Shared.Dtos;
using Inventory.Web.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Inventory.Web.Controllers;

[Authorize]
public class CategoriesController : Controller
{
    private readonly IInventoryApiClient _api;

    public CategoriesController(IInventoryApiClient api) => _api = api;

    public async Task<IActionResult> Index(string? search)
    {
        ViewBag.Search = search;
        var list = await _api.GetCategoriesAsync(search);
        return View(list);
    }

    public async Task<IActionResult> Details(int id)
    {
        var dto = await _api.GetCategoryAsync(id);
        if (dto is null) return NotFound();
        return View(dto);
    }

    [Authorize(Roles = IdentitySeeder.AdminRole)]
    public IActionResult Create() => View(new CategoryDto());

    [HttpPost, ValidateAntiForgeryToken]
    [Authorize(Roles = IdentitySeeder.AdminRole)]
    public async Task<IActionResult> Create(CategoryDto dto)
    {
        if (!ModelState.IsValid) return View(dto);
        var (ok, error) = await _api.CreateCategoryAsync(dto);
        if (!ok)
        {
            ModelState.AddModelError(string.Empty, error ?? "Failed to create category.");
            return View(dto);
        }
        TempData["Success"] = $"Category '{dto.Name}' created.";
        return RedirectToAction(nameof(Index));
    }

    [Authorize(Roles = IdentitySeeder.AdminRole)]
    public async Task<IActionResult> Edit(int id)
    {
        var dto = await _api.GetCategoryAsync(id);
        if (dto is null) return NotFound();
        return View(dto);
    }

    [HttpPost, ValidateAntiForgeryToken]
    [Authorize(Roles = IdentitySeeder.AdminRole)]
    public async Task<IActionResult> Edit(int id, CategoryDto dto)
    {
        if (id != dto.Id) return BadRequest();
        if (!ModelState.IsValid) return View(dto);
        var (ok, error) = await _api.UpdateCategoryAsync(dto);
        if (!ok)
        {
            ModelState.AddModelError(string.Empty, error ?? "Failed to update category.");
            return View(dto);
        }
        TempData["Success"] = $"Category '{dto.Name}' updated.";
        return RedirectToAction(nameof(Index));
    }

    [Authorize(Roles = IdentitySeeder.AdminRole)]
    public async Task<IActionResult> Delete(int id)
    {
        var dto = await _api.GetCategoryAsync(id);
        if (dto is null) return NotFound();
        return View(dto);
    }

    [HttpPost, ActionName("Delete"), ValidateAntiForgeryToken]
    [Authorize(Roles = IdentitySeeder.AdminRole)]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var (ok, error) = await _api.DeleteCategoryAsync(id);
        if (!ok)
        {
            TempData["Error"] = error ?? "Failed to delete category.";
            return RedirectToAction(nameof(Delete), new { id });
        }
        TempData["Success"] = "Category deleted.";
        return RedirectToAction(nameof(Index));
    }
}
