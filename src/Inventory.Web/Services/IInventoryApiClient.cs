using Inventory.Shared.Dtos;

namespace Inventory.Web.Services;

public interface IInventoryApiClient
{
    Task<IList<CategoryDto>> GetCategoriesAsync(string? search = null);
    Task<CategoryDto?> GetCategoryAsync(int id);
    Task<(bool Ok, string? Error)> CreateCategoryAsync(CategoryDto dto);
    Task<(bool Ok, string? Error)> UpdateCategoryAsync(CategoryDto dto);
    Task<(bool Ok, string? Error)> DeleteCategoryAsync(int id);

    Task<IList<SupplierDto>> GetSuppliersAsync(string? search = null);
    Task<SupplierDto?> GetSupplierAsync(int id);
    Task<(bool Ok, string? Error)> CreateSupplierAsync(SupplierDto dto);
    Task<(bool Ok, string? Error)> UpdateSupplierAsync(SupplierDto dto);
    Task<(bool Ok, string? Error)> DeleteSupplierAsync(int id);

    Task<IList<ProductDto>> GetProductsAsync(string? search = null, int? categoryId = null, int? supplierId = null, bool? lowStock = null);
    Task<ProductDto?> GetProductAsync(int id);
    Task<(bool Ok, string? Error)> CreateProductAsync(ProductDto dto);
    Task<(bool Ok, string? Error)> UpdateProductAsync(ProductDto dto);
    Task<(bool Ok, string? Error)> DeleteProductAsync(int id);

    Task<IList<StockTransactionDto>> GetStockTransactionsAsync(int? productId = null, StockMovementType? type = null, int take = 100);
    Task<(bool Ok, string? Error)> CreateStockTransactionAsync(StockTransactionDto dto);
    Task<(bool Ok, string? Error)> DeleteStockTransactionAsync(int id);

    Task<DashboardDto?> GetDashboardAsync();
}
