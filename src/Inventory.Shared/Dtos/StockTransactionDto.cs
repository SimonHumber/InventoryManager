using System.ComponentModel.DataAnnotations;

namespace Inventory.Shared.Dtos;

public enum StockMovementType
{
    Inbound = 0,
    Outbound = 1,
    Adjustment = 2
}

public class StockTransactionDto
{
    public int Id { get; set; }

    public int ProductId { get; set; }

    public string? ProductName { get; set; }

    public string? ProductSku { get; set; }

    [Required]
    public StockMovementType MovementType { get; set; }

    [Range(1, 100_000)]
    public int Quantity { get; set; }

    [StringLength(250)]
    public string? Note { get; set; }

    [StringLength(80)]
    public string? PerformedBy { get; set; }

    public DateTime OccurredAt { get; set; }
}
