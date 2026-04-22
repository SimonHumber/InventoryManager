using System.ComponentModel.DataAnnotations;

namespace Inventory.Shared.Dtos;

public class CategoryDto
{
    public int Id { get; set; }

    [Required, StringLength(60, MinimumLength = 2)]
    public string Name { get; set; } = string.Empty;

    [StringLength(250)]
    public string? Description { get; set; }

    public int ProductCount { get; set; }
}
