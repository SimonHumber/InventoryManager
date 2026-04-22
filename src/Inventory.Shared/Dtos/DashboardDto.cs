namespace Inventory.Shared.Dtos;

public class DashboardDto
{
    public int TotalProducts { get; set; }
    public int TotalCategories { get; set; }
    public int TotalSuppliers { get; set; }
    public int TotalStockUnits { get; set; }
    public int LowStockCount { get; set; }
    public decimal InventoryValue { get; set; }
    public List<ProductDto> LowStockProducts { get; set; } = new();
    public List<StockTransactionDto> RecentTransactions { get; set; } = new();
}
