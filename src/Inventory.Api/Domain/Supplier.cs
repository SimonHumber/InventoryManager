using System.ComponentModel.DataAnnotations;

namespace Inventory.Api.Domain;

public class Supplier
{
    public int Id { get; set; }

    [Required, StringLength(80)]
    public string Name { get; set; } = string.Empty;

    [StringLength(80)]
    public string? ContactName { get; set; }

    [StringLength(120)]
    public string? Email { get; set; }

    [StringLength(40)]
    public string? Phone { get; set; }

    [StringLength(200)]
    public string? Address { get; set; }

    public ICollection<Product> Products { get; set; } = new List<Product>();
}
