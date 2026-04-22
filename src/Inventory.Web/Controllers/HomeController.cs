using System.Diagnostics;
using Inventory.Shared.Dtos;
using Inventory.Web.Models;
using Inventory.Web.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Inventory.Web.Controllers;

public class HomeController : Controller
{
    private readonly IInventoryApiClient _api;

    public HomeController(IInventoryApiClient api) => _api = api;

    public IActionResult Index() => View();

    [Authorize]
    public async Task<IActionResult> Dashboard()
    {
        var data = await _api.GetDashboardAsync() ?? new DashboardDto();
        return View(data);
    }

    public IActionResult AccessDenied() => View();

    public IActionResult Privacy() => View();

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
