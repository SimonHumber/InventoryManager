using System.ComponentModel.DataAnnotations;
using Inventory.Shared.Dtos;

namespace Inventory.Api.Domain;

public class StockTransaction
{
    public int Id { get; set; }

    public int ProductId { get; set; }
    public Product? Product { get; set; }

    [Required]
    public StockMovementType MovementType { get; set; }

    public int Quantity { get; set; }

    [StringLength(250)]
    public string? Note { get; set; }

    [StringLength(80)]
    public string? PerformedBy { get; set; }

    public DateTime OccurredAt { get; set; } = DateTime.UtcNow;
}
