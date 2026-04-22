using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Inventory.Api.Data.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Categories",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", maxLength: 60, nullable: false),
                    Description = table.Column<string>(type: "TEXT", maxLength: 250, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Categories", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Suppliers",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", maxLength: 80, nullable: false),
                    ContactName = table.Column<string>(type: "TEXT", maxLength: 80, nullable: true),
                    Email = table.Column<string>(type: "TEXT", maxLength: 120, nullable: true),
                    Phone = table.Column<string>(type: "TEXT", maxLength: 40, nullable: true),
                    Address = table.Column<string>(type: "TEXT", maxLength: 200, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Suppliers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Products",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Sku = table.Column<string>(type: "TEXT", maxLength: 60, nullable: false),
                    Name = table.Column<string>(type: "TEXT", maxLength: 120, nullable: false),
                    Description = table.Column<string>(type: "TEXT", maxLength: 500, nullable: true),
                    UnitPrice = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    QuantityInStock = table.Column<int>(type: "INTEGER", nullable: false),
                    ReorderLevel = table.Column<int>(type: "INTEGER", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    CategoryId = table.Column<int>(type: "INTEGER", nullable: false),
                    SupplierId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Products", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Products_Categories_CategoryId",
                        column: x => x.CategoryId,
                        principalTable: "Categories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Products_Suppliers_SupplierId",
                        column: x => x.SupplierId,
                        principalTable: "Suppliers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "StockTransactions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    ProductId = table.Column<int>(type: "INTEGER", nullable: false),
                    MovementType = table.Column<int>(type: "INTEGER", nullable: false),
                    Quantity = table.Column<int>(type: "INTEGER", nullable: false),
                    Note = table.Column<string>(type: "TEXT", maxLength: 250, nullable: true),
                    PerformedBy = table.Column<string>(type: "TEXT", maxLength: 80, nullable: true),
                    OccurredAt = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StockTransactions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_StockTransactions_Products_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Products",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Categories",
                columns: new[] { "Id", "Description", "Name" },
                values: new object[,]
                {
                    { 1, "Portable computers and notebooks", "Laptops" },
                    { 2, "Desktop computers and workstations", "Desktops" },
                    { 3, "External displays and monitors", "Monitors" },
                    { 4, "Mechanical and membrane keyboards", "Keyboards" },
                    { 5, "Wired and wireless pointing devices", "Mice" },
                    { 6, "Routers, switches, cables", "Networking" },
                    { 7, "Hard drives, SSDs, USB drives", "Storage" },
                    { 8, "Laser and inkjet printers", "Printers" },
                    { 9, "Cables, adapters, stands", "Accessories" },
                    { 10, "Headphones, speakers, microphones", "Audio" }
                });

            migrationBuilder.InsertData(
                table: "Suppliers",
                columns: new[] { "Id", "Address", "ContactName", "Email", "Name", "Phone" },
                values: new object[,]
                {
                    { 1, "120 King St W, Toronto, ON", "Alice Nguyen", "sales@techworld.example", "TechWorld Distributors", "+1-416-555-0101" },
                    { 2, "88 Queen St E, Toronto, ON", "Raj Patel", "orders@bytemart.example", "ByteMart Wholesale", "+1-416-555-0102" },
                    { 3, "45 Main St, Mississauga, ON", "Emily Chen", "hello@pixelpro.example", "PixelPro Supply", "+1-905-555-0103" },
                    { 4, "210 Industrial Rd, Brampton, ON", "David Kim", "ops@netgearlog.example", "NetGear Logistics", "+1-905-555-0104" },
                    { 5, "300 Bay St, Toronto, ON", "Sara Lopez", "info@corecircuit.example", "CoreCircuit Parts", "+1-647-555-0105" },
                    { 6, "14 Bloor St W, Toronto, ON", "Michael Brown", "contact@synapse.example", "Synapse Electronics", "+1-647-555-0106" },
                    { 7, "500 University Ave, Toronto, ON", "Olivia Wilson", "sales@northstarit.example", "NorthStar IT Supply", "+1-416-555-0107" },
                    { 8, "22 Dundas Sq, Toronto, ON", "Liam O'Connor", "support@quantumg.example", "Quantum Gadgets", "+1-416-555-0108" },
                    { 9, "77 Yonge St, Richmond Hill, ON", "Aisha Khan", "orders@orbitoffice.example", "Orbit Office Supply", "+1-905-555-0109" },
                    { 10, "99 Dufferin St, Vaughan, ON", "Noah Smith", "hello@peakperi.example", "Peak Peripherals", "+1-905-555-0110" }
                });

            migrationBuilder.InsertData(
                table: "Products",
                columns: new[] { "Id", "CategoryId", "CreatedAt", "Description", "Name", "QuantityInStock", "ReorderLevel", "Sku", "SupplierId", "UnitPrice" },
                values: new object[,]
                {
                    { 1, 1, new DateTime(2026, 1, 15, 9, 0, 0, 0, DateTimeKind.Utc), "14-inch ultrabook, 16GB RAM, 512GB SSD", "Acme Pro 14 Laptop", 25, 5, "LAP-001", 1, 1299.00m },
                    { 2, 1, new DateTime(2026, 1, 15, 9, 0, 0, 0, DateTimeKind.Utc), "15-inch gaming laptop, RTX GPU", "Acme Gamer 15 Laptop", 12, 4, "LAP-002", 2, 1899.00m },
                    { 3, 2, new DateTime(2026, 1, 15, 9, 0, 0, 0, DateTimeKind.Utc), "Compact desktop for office use", "WorkStation Mini Tower", 18, 5, "DSK-001", 1, 899.00m },
                    { 4, 3, new DateTime(2026, 1, 15, 9, 0, 0, 0, DateTimeKind.Utc), "27-inch 4K UHD IPS display", "27\" 4K IPS Monitor", 30, 8, "MON-001", 3, 549.00m },
                    { 5, 3, new DateTime(2026, 1, 15, 9, 0, 0, 0, DateTimeKind.Utc), "24-inch 1080p office display", "24\" FHD Business Monitor", 40, 10, "MON-002", 3, 199.00m },
                    { 6, 4, new DateTime(2026, 1, 15, 9, 0, 0, 0, DateTimeKind.Utc), "Mechanical keyboard with RGB backlight", "Mechanical RGB Keyboard", 55, 12, "KBD-001", 5, 129.00m },
                    { 7, 4, new DateTime(2026, 1, 15, 9, 0, 0, 0, DateTimeKind.Utc), "Bluetooth low-profile keyboard", "Wireless Slim Keyboard", 3, 10, "KBD-002", 10, 79.00m },
                    { 8, 5, new DateTime(2026, 1, 15, 9, 0, 0, 0, DateTimeKind.Utc), "Ergonomic wireless mouse", "Wireless Ergonomic Mouse", 80, 15, "MSE-001", 10, 39.00m },
                    { 9, 6, new DateTime(2026, 1, 15, 9, 0, 0, 0, DateTimeKind.Utc), "Dual-band Wi-Fi 6 router, 4 LAN ports", "Wi-Fi 6 Router", 22, 5, "NET-001", 4, 179.00m },
                    { 10, 6, new DateTime(2026, 1, 15, 9, 0, 0, 0, DateTimeKind.Utc), "Unmanaged 24-port gigabit switch", "24-Port Gigabit Switch", 8, 3, "NET-002", 4, 219.00m },
                    { 11, 7, new DateTime(2026, 1, 15, 9, 0, 0, 0, DateTimeKind.Utc), "M.2 NVMe SSD 1TB", "1TB NVMe SSD", 45, 10, "STO-001", 5, 119.00m },
                    { 12, 7, new DateTime(2026, 1, 15, 9, 0, 0, 0, DateTimeKind.Utc), "USB 3.0 portable hard drive", "2TB External HDD", 35, 8, "STO-002", 7, 89.00m },
                    { 13, 8, new DateTime(2026, 1, 15, 9, 0, 0, 0, DateTimeKind.Utc), "Wi-Fi color laser printer", "Color Laser Printer", 6, 2, "PRN-001", 9, 329.00m },
                    { 14, 9, new DateTime(2026, 1, 15, 9, 0, 0, 0, DateTimeKind.Utc), "7-port USB-C hub with HDMI", "USB-C Hub 7-in-1", 120, 20, "ACC-001", 8, 49.00m },
                    { 15, 10, new DateTime(2026, 1, 15, 9, 0, 0, 0, DateTimeKind.Utc), "Over-ear ANC headphones", "Noise-Cancelling Headset", 15, 5, "AUD-001", 6, 249.00m }
                });

            migrationBuilder.InsertData(
                table: "StockTransactions",
                columns: new[] { "Id", "MovementType", "Note", "OccurredAt", "PerformedBy", "ProductId", "Quantity" },
                values: new object[,]
                {
                    { 1, 0, "Initial shipment", new DateTime(2026, 2, 1, 10, 0, 0, 0, DateTimeKind.Utc), "seed", 1, 20 },
                    { 2, 1, "Order #1001", new DateTime(2026, 2, 2, 10, 0, 0, 0, DateTimeKind.Utc), "seed", 1, 5 },
                    { 3, 0, "Initial shipment", new DateTime(2026, 2, 1, 10, 0, 0, 0, DateTimeKind.Utc), "seed", 2, 15 },
                    { 4, 0, "Initial shipment", new DateTime(2026, 2, 1, 10, 0, 0, 0, DateTimeKind.Utc), "seed", 3, 20 },
                    { 5, 0, "Initial shipment", new DateTime(2026, 2, 1, 10, 0, 0, 0, DateTimeKind.Utc), "seed", 4, 30 },
                    { 6, 1, "Bulk order", new DateTime(2026, 2, 3, 10, 0, 0, 0, DateTimeKind.Utc), "seed", 5, 10 },
                    { 7, 0, "Restock", new DateTime(2026, 2, 1, 10, 0, 0, 0, DateTimeKind.Utc), "seed", 6, 60 },
                    { 8, 1, "Online order", new DateTime(2026, 2, 4, 10, 0, 0, 0, DateTimeKind.Utc), "seed", 7, 7 },
                    { 9, 2, "Damaged stock write-off", new DateTime(2026, 2, 5, 10, 0, 0, 0, DateTimeKind.Utc), "seed", 8, 2 },
                    { 10, 0, "Initial shipment", new DateTime(2026, 2, 1, 10, 0, 0, 0, DateTimeKind.Utc), "seed", 9, 25 },
                    { 11, 0, "Restock", new DateTime(2026, 2, 2, 10, 0, 0, 0, DateTimeKind.Utc), "seed", 11, 50 },
                    { 12, 0, "Bulk restock", new DateTime(2026, 2, 1, 10, 0, 0, 0, DateTimeKind.Utc), "seed", 14, 120 }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Categories_Name",
                table: "Categories",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Products_CategoryId",
                table: "Products",
                column: "CategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_Products_Sku",
                table: "Products",
                column: "Sku",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Products_SupplierId",
                table: "Products",
                column: "SupplierId");

            migrationBuilder.CreateIndex(
                name: "IX_StockTransactions_ProductId",
                table: "StockTransactions",
                column: "ProductId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "StockTransactions");

            migrationBuilder.DropTable(
                name: "Products");

            migrationBuilder.DropTable(
                name: "Categories");

            migrationBuilder.DropTable(
                name: "Suppliers");
        }
    }
}
