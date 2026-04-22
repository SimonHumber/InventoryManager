using System.ComponentModel.DataAnnotations;

namespace Inventory.Shared.Dtos;

public class SupplierDto
{
    public int Id { get; set; }

    [Required, StringLength(80, MinimumLength = 2)]
    public string Name { get; set; } = string.Empty;

    [StringLength(80)]
    public string? ContactName { get; set; }

    [EmailAddress, StringLength(120)]
    public string? Email { get; set; }

    [Phone, StringLength(40)]
    public string? Phone { get; set; }

    [StringLength(200)]
    public string? Address { get; set; }
}
