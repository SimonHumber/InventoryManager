using Inventory.Api.Domain;
using Inventory.Shared.Dtos;
using Microsoft.EntityFrameworkCore;

namespace Inventory.Api.Data;

public class InventoryDbContext : DbContext
{
    public InventoryDbContext(DbContextOptions<InventoryDbContext> options) : base(options) { }

    public DbSet<Category> Categories => Set<Category>();
    public DbSet<Supplier> Suppliers => Set<Supplier>();
    public DbSet<Product> Products => Set<Product>();
    public DbSet<StockTransaction> StockTransactions => Set<StockTransaction>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Category>()
            .HasIndex(c => c.Name)
            .IsUnique();

        modelBuilder.Entity<Product>()
            .HasIndex(p => p.Sku)
            .IsUnique();

        modelBuilder.Entity<Product>()
            .HasOne(p => p.Category)
            .WithMany(c => c.Products)
            .HasForeignKey(p => p.CategoryId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Product>()
            .HasOne(p => p.Supplier)
            .WithMany(s => s.Products)
            .HasForeignKey(p => p.SupplierId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<StockTransaction>()
            .HasOne(t => t.Product)
            .WithMany(p => p.StockTransactions)
            .HasForeignKey(t => t.ProductId)
            .OnDelete(DeleteBehavior.Cascade);

        SeedData(modelBuilder);
    }

    private static void SeedData(ModelBuilder modelBuilder)
    {
        var categories = new[]
        {
            new Category { Id = 1, Name = "Laptops", Description = "Portable computers and notebooks" },
            new Category { Id = 2, Name = "Desktops", Description = "Desktop computers and workstations" },
            new Category { Id = 3, Name = "Monitors", Description = "External displays and monitors" },
            new Category { Id = 4, Name = "Keyboards", Description = "Mechanical and membrane keyboards" },
            new Category { Id = 5, Name = "Mice", Description = "Wired and wireless pointing devices" },
            new Category { Id = 6, Name = "Networking", Description = "Routers, switches, cables" },
            new Category { Id = 7, Name = "Storage", Description = "Hard drives, SSDs, USB drives" },
            new Category { Id = 8, Name = "Printers", Description = "Laser and inkjet printers" },
            new Category { Id = 9, Name = "Accessories", Description = "Cables, adapters, stands" },
            new Category { Id = 10, Name = "Audio", Description = "Headphones, speakers, microphones" }
        };

        var suppliers = new[]
        {
            new Supplier { Id = 1, Name = "TechWorld Distributors", ContactName = "Alice Nguyen", Email = "sales@techworld.example", Phone = "+1-416-555-0101", Address = "120 King St W, Toronto, ON" },
            new Supplier { Id = 2, Name = "ByteMart Wholesale", ContactName = "Raj Patel", Email = "orders@bytemart.example", Phone = "+1-416-555-0102", Address = "88 Queen St E, Toronto, ON" },
            new Supplier { Id = 3, Name = "PixelPro Supply", ContactName = "Emily Chen", Email = "hello@pixelpro.example", Phone = "+1-905-555-0103", Address = "45 Main St, Mississauga, ON" },
            new Supplier { Id = 4, Name = "NetGear Logistics", ContactName = "David Kim", Email = "ops@netgearlog.example", Phone = "+1-905-555-0104", Address = "210 Industrial Rd, Brampton, ON" },
            new Supplier { Id = 5, Name = "CoreCircuit Parts", ContactName = "Sara Lopez", Email = "info@corecircuit.example", Phone = "+1-647-555-0105", Address = "300 Bay St, Toronto, ON" },
            new Supplier { Id = 6, Name = "Synapse Electronics", ContactName = "Michael Brown", Email = "contact@synapse.example", Phone = "+1-647-555-0106", Address = "14 Bloor St W, Toronto, ON" },
            new Supplier { Id = 7, Name = "NorthStar IT Supply", ContactName = "Olivia Wilson", Email = "sales@northstarit.example", Phone = "+1-416-555-0107", Address = "500 University Ave, Toronto, ON" },
            new Supplier { Id = 8, Name = "Quantum Gadgets", ContactName = "Liam O'Connor", Email = "support@quantumg.example", Phone = "+1-416-555-0108", Address = "22 Dundas Sq, Toronto, ON" },
            new Supplier { Id = 9, Name = "Orbit Office Supply", ContactName = "Aisha Khan", Email = "orders@orbitoffice.example", Phone = "+1-905-555-0109", Address = "77 Yonge St, Richmond Hill, ON" },
            new Supplier { Id = 10, Name = "Peak Peripherals", ContactName = "Noah Smith", Email = "hello@peakperi.example", Phone = "+1-905-555-0110", Address = "99 Dufferin St, Vaughan, ON" }
        };

        var created = new DateTime(2026, 1, 15, 9, 0, 0, DateTimeKind.Utc);
        var products = new[]
        {
            new Product { Id = 1,  Sku = "LAP-001", Name = "Acme Pro 14 Laptop",      Description = "14-inch ultrabook, 16GB RAM, 512GB SSD", UnitPrice = 1299.00m, QuantityInStock = 25, ReorderLevel = 5,  CategoryId = 1,  SupplierId = 1,  CreatedAt = created },
            new Product { Id = 2,  Sku = "LAP-002", Name = "Acme Gamer 15 Laptop",    Description = "15-inch gaming laptop, RTX GPU",           UnitPrice = 1899.00m, QuantityInStock = 12, ReorderLevel = 4,  CategoryId = 1,  SupplierId = 2,  CreatedAt = created },
            new Product { Id = 3,  Sku = "DSK-001", Name = "WorkStation Mini Tower",  Description = "Compact desktop for office use",           UnitPrice = 899.00m,  QuantityInStock = 18, ReorderLevel = 5,  CategoryId = 2,  SupplierId = 1,  CreatedAt = created },
            new Product { Id = 4,  Sku = "MON-001", Name = "27\" 4K IPS Monitor",     Description = "27-inch 4K UHD IPS display",                UnitPrice = 549.00m,  QuantityInStock = 30, ReorderLevel = 8,  CategoryId = 3,  SupplierId = 3,  CreatedAt = created },
            new Product { Id = 5,  Sku = "MON-002", Name = "24\" FHD Business Monitor", Description = "24-inch 1080p office display",            UnitPrice = 199.00m,  QuantityInStock = 40, ReorderLevel = 10, CategoryId = 3,  SupplierId = 3,  CreatedAt = created },
            new Product { Id = 6,  Sku = "KBD-001", Name = "Mechanical RGB Keyboard", Description = "Mechanical keyboard with RGB backlight",    UnitPrice = 129.00m,  QuantityInStock = 55, ReorderLevel = 12, CategoryId = 4,  SupplierId = 5,  CreatedAt = created },
            new Product { Id = 7,  Sku = "KBD-002", Name = "Wireless Slim Keyboard",  Description = "Bluetooth low-profile keyboard",            UnitPrice = 79.00m,   QuantityInStock = 3,  ReorderLevel = 10, CategoryId = 4,  SupplierId = 10, CreatedAt = created },
            new Product { Id = 8,  Sku = "MSE-001", Name = "Wireless Ergonomic Mouse",Description = "Ergonomic wireless mouse",                 UnitPrice = 39.00m,   QuantityInStock = 80, ReorderLevel = 15, CategoryId = 5,  SupplierId = 10, CreatedAt = created },
            new Product { Id = 9,  Sku = "NET-001", Name = "Wi-Fi 6 Router",          Description = "Dual-band Wi-Fi 6 router, 4 LAN ports",     UnitPrice = 179.00m,  QuantityInStock = 22, ReorderLevel = 5,  CategoryId = 6,  SupplierId = 4,  CreatedAt = created },
            new Product { Id = 10, Sku = "NET-002", Name = "24-Port Gigabit Switch",  Description = "Unmanaged 24-port gigabit switch",         UnitPrice = 219.00m,  QuantityInStock = 8,  ReorderLevel = 3,  CategoryId = 6,  SupplierId = 4,  CreatedAt = created },
            new Product { Id = 11, Sku = "STO-001", Name = "1TB NVMe SSD",             Description = "M.2 NVMe SSD 1TB",                         UnitPrice = 119.00m,  QuantityInStock = 45, ReorderLevel = 10, CategoryId = 7,  SupplierId = 5,  CreatedAt = created },
            new Product { Id = 12, Sku = "STO-002", Name = "2TB External HDD",         Description = "USB 3.0 portable hard drive",              UnitPrice = 89.00m,   QuantityInStock = 35, ReorderLevel = 8,  CategoryId = 7,  SupplierId = 7,  CreatedAt = created },
            new Product { Id = 13, Sku = "PRN-001", Name = "Color Laser Printer",      Description = "Wi-Fi color laser printer",                UnitPrice = 329.00m,  QuantityInStock = 6,  ReorderLevel = 2,  CategoryId = 8,  SupplierId = 9,  CreatedAt = created },
            new Product { Id = 14, Sku = "ACC-001", Name = "USB-C Hub 7-in-1",         Description = "7-port USB-C hub with HDMI",               UnitPrice = 49.00m,   QuantityInStock = 120,ReorderLevel = 20, CategoryId = 9,  SupplierId = 8,  CreatedAt = created },
            new Product { Id = 15, Sku = "AUD-001", Name = "Noise-Cancelling Headset", Description = "Over-ear ANC headphones",                  UnitPrice = 249.00m,  QuantityInStock = 15, ReorderLevel = 5,  CategoryId = 10, SupplierId = 6,  CreatedAt = created }
        };

        var tx = new DateTime(2026, 2, 1, 10, 0, 0, DateTimeKind.Utc);
        var transactions = new[]
        {
            new StockTransaction { Id = 1,  ProductId = 1,  MovementType = StockMovementType.Inbound,   Quantity = 20, Note = "Initial shipment",       PerformedBy = "seed", OccurredAt = tx },
            new StockTransaction { Id = 2,  ProductId = 1,  MovementType = StockMovementType.Outbound,  Quantity = 5,  Note = "Order #1001",             PerformedBy = "seed", OccurredAt = tx.AddDays(1) },
            new StockTransaction { Id = 3,  ProductId = 2,  MovementType = StockMovementType.Inbound,   Quantity = 15, Note = "Initial shipment",       PerformedBy = "seed", OccurredAt = tx },
            new StockTransaction { Id = 4,  ProductId = 3,  MovementType = StockMovementType.Inbound,   Quantity = 20, Note = "Initial shipment",       PerformedBy = "seed", OccurredAt = tx },
            new StockTransaction { Id = 5,  ProductId = 4,  MovementType = StockMovementType.Inbound,   Quantity = 30, Note = "Initial shipment",       PerformedBy = "seed", OccurredAt = tx },
            new StockTransaction { Id = 6,  ProductId = 5,  MovementType = StockMovementType.Outbound,  Quantity = 10, Note = "Bulk order",              PerformedBy = "seed", OccurredAt = tx.AddDays(2) },
            new StockTransaction { Id = 7,  ProductId = 6,  MovementType = StockMovementType.Inbound,   Quantity = 60, Note = "Restock",                 PerformedBy = "seed", OccurredAt = tx },
            new StockTransaction { Id = 8,  ProductId = 7,  MovementType = StockMovementType.Outbound,  Quantity = 7,  Note = "Online order",            PerformedBy = "seed", OccurredAt = tx.AddDays(3) },
            new StockTransaction { Id = 9,  ProductId = 8,  MovementType = StockMovementType.Adjustment,Quantity = 2,  Note = "Damaged stock write-off", PerformedBy = "seed", OccurredAt = tx.AddDays(4) },
            new StockTransaction { Id = 10, ProductId = 9,  MovementType = StockMovementType.Inbound,   Quantity = 25, Note = "Initial shipment",       PerformedBy = "seed", OccurredAt = tx },
            new StockTransaction { Id = 11, ProductId = 11, MovementType = StockMovementType.Inbound,   Quantity = 50, Note = "Restock",                 PerformedBy = "seed", OccurredAt = tx.AddDays(1) },
            new StockTransaction { Id = 12, ProductId = 14, MovementType = StockMovementType.Inbound,   Quantity = 120,Note = "Bulk restock",            PerformedBy = "seed", OccurredAt = tx }
        };

        modelBuilder.Entity<Category>().HasData(categories);
        modelBuilder.Entity<Supplier>().HasData(suppliers);
        modelBuilder.Entity<Product>().HasData(products);
        modelBuilder.Entity<StockTransaction>().HasData(transactions);
    }
}
