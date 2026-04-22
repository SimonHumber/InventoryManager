using System.Net.Http.Json;
using System.Text.Json;
using Inventory.Shared.Dtos;
using Microsoft.AspNetCore.WebUtilities;

namespace Inventory.Web.Services;

public class InventoryApiClient : IInventoryApiClient
{
    private readonly HttpClient _http;
    private readonly ILogger<InventoryApiClient> _logger;
    private static readonly JsonSerializerOptions JsonOpts = new(JsonSerializerDefaults.Web);

    public InventoryApiClient(HttpClient http, ILogger<InventoryApiClient> logger)
    {
        _http = http;
        _logger = logger;
    }

    private static string BuildQuery(string path, IDictionary<string, string?> parameters)
    {
        var clean = parameters
            .Where(kv => !string.IsNullOrWhiteSpace(kv.Value))
            .ToDictionary(kv => kv.Key, kv => (string?)kv.Value);
        return clean.Count == 0 ? path : QueryHelpers.AddQueryString(path, clean);
    }

    private async Task<(bool Ok, string? Error)> SendAsync(HttpMethod method, string url, object? body = null)
    {
        try
        {
            using var request = new HttpRequestMessage(method, url);
            if (body is not null)
                request.Content = JsonContent.Create(body, options: JsonOpts);
            using var response = await _http.SendAsync(request);
            if (response.IsSuccessStatusCode) return (true, null);

            var text = await response.Content.ReadAsStringAsync();
            string? message = null;
            try
            {
                using var doc = JsonDocument.Parse(text);
                if (doc.RootElement.TryGetProperty("message", out var m))
                    message = m.GetString();
                else if (doc.RootElement.TryGetProperty("title", out var t))
                    message = t.GetString();
            }
            catch (JsonException) { /* not JSON */ }

            return (false, message ?? $"Request failed with status {(int)response.StatusCode}.");
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "API request failed: {Url}", url);
            return (false, "Unable to reach the inventory microservice. Is it running?");
        }
    }

    private async Task<T?> GetJsonAsync<T>(string url)
    {
        try
        {
            return await _http.GetFromJsonAsync<T>(url, JsonOpts);
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "API GET failed: {Url}", url);
            return default;
        }
    }

    public async Task<IList<CategoryDto>> GetCategoriesAsync(string? search = null)
    {
        var url = BuildQuery("api/categories", new Dictionary<string, string?> { ["search"] = search });
        var list = await GetJsonAsync<List<CategoryDto>>(url);
        return list ?? new List<CategoryDto>();
    }

    public Task<CategoryDto?> GetCategoryAsync(int id) => GetJsonAsync<CategoryDto>($"api/categories/{id}");

    public Task<(bool Ok, string? Error)> CreateCategoryAsync(CategoryDto dto) =>
        SendAsync(HttpMethod.Post, "api/categories", dto);

    public Task<(bool Ok, string? Error)> UpdateCategoryAsync(CategoryDto dto) =>
        SendAsync(HttpMethod.Put, $"api/categories/{dto.Id}", dto);

    public Task<(bool Ok, string? Error)> DeleteCategoryAsync(int id) =>
        SendAsync(HttpMethod.Delete, $"api/categories/{id}");

    public async Task<IList<SupplierDto>> GetSuppliersAsync(string? search = null)
    {
        var url = BuildQuery("api/suppliers", new Dictionary<string, string?> { ["search"] = search });
        var list = await GetJsonAsync<List<SupplierDto>>(url);
        return list ?? new List<SupplierDto>();
    }

    public Task<SupplierDto?> GetSupplierAsync(int id) => GetJsonAsync<SupplierDto>($"api/suppliers/{id}");

    public Task<(bool Ok, string? Error)> CreateSupplierAsync(SupplierDto dto) =>
        SendAsync(HttpMethod.Post, "api/suppliers", dto);

    public Task<(bool Ok, string? Error)> UpdateSupplierAsync(SupplierDto dto) =>
        SendAsync(HttpMethod.Put, $"api/suppliers/{dto.Id}", dto);

    public Task<(bool Ok, string? Error)> DeleteSupplierAsync(int id) =>
        SendAsync(HttpMethod.Delete, $"api/suppliers/{id}");

    public async Task<IList<ProductDto>> GetProductsAsync(string? search = null, int? categoryId = null, int? supplierId = null, bool? lowStock = null)
    {
        var url = BuildQuery("api/products", new Dictionary<string, string?>
        {
            ["search"] = search,
            ["categoryId"] = categoryId?.ToString(),
            ["supplierId"] = supplierId?.ToString(),
            ["lowStock"] = lowStock?.ToString()
        });
        var list = await GetJsonAsync<List<ProductDto>>(url);
        return list ?? new List<ProductDto>();
    }

    public Task<ProductDto?> GetProductAsync(int id) => GetJsonAsync<ProductDto>($"api/products/{id}");

    public Task<(bool Ok, string? Error)> CreateProductAsync(ProductDto dto) =>
        SendAsync(HttpMethod.Post, "api/products", dto);

    public Task<(bool Ok, string? Error)> UpdateProductAsync(ProductDto dto) =>
        SendAsync(HttpMethod.Put, $"api/products/{dto.Id}", dto);

    public Task<(bool Ok, string? Error)> DeleteProductAsync(int id) =>
        SendAsync(HttpMethod.Delete, $"api/products/{id}");

    public async Task<IList<StockTransactionDto>> GetStockTransactionsAsync(int? productId = null, StockMovementType? type = null, int take = 100)
    {
        var url = BuildQuery("api/stock", new Dictionary<string, string?>
        {
            ["productId"] = productId?.ToString(),
            ["type"] = type?.ToString(),
            ["take"] = take.ToString()
        });
        var list = await GetJsonAsync<List<StockTransactionDto>>(url);
        return list ?? new List<StockTransactionDto>();
    }

    public Task<(bool Ok, string? Error)> CreateStockTransactionAsync(StockTransactionDto dto) =>
        SendAsync(HttpMethod.Post, "api/stock", dto);

    public Task<(bool Ok, string? Error)> DeleteStockTransactionAsync(int id) =>
        SendAsync(HttpMethod.Delete, $"api/stock/{id}");

    public Task<DashboardDto?> GetDashboardAsync() => GetJsonAsync<DashboardDto>("api/dashboard");
}
