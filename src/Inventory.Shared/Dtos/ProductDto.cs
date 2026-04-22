using System.ComponentModel.DataAnnotations;

namespace Inventory.Shared.Dtos;

public class ProductDto
{
    public int Id { get; set; }

    [Required, StringLength(60, MinimumLength = 2)]
    public string Sku { get; set; } = string.Empty;

    [Required, StringLength(120, MinimumLength = 2)]
    public string Name { get; set; } = string.Empty;

    [StringLength(500)]
    public string? Description { get; set; }

    [Range(0.0, 1_000_000.0)]
    public decimal UnitPrice { get; set; }

    [Range(0, 1_000_000)]
    public int QuantityInStock { get; set; }

    [Range(0, 10_000)]
    public int ReorderLevel { get; set; }

    public int CategoryId { get; set; }

    public string? CategoryName { get; set; }

    public int SupplierId { get; set; }

    public string? SupplierName { get; set; }

    public DateTime CreatedAt { get; set; }
}
