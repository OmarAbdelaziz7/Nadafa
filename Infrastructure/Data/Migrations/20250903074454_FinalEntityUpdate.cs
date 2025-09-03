using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Infrastructure.Data.Migrations
{
    public partial class FinalEntityUpdate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Note: Commenting out foreign key drops that don't exist in the current database
            // migrationBuilder.DropForeignKey(
            //     name: "FK_MarketplaceItems_Factories_PurchasedByFactoryId",
            //     table: "MarketplaceItems");

            // migrationBuilder.DropForeignKey(
            //     name: "FK_PickupRequests_Users_ApprovedByAdminId",
            //     table: "PickupRequests");

            // Note: Commenting out index drops that might not exist in the current database
            // migrationBuilder.DropIndex(
            //     name: "IX_PickupRequests_ApprovedByAdminId",
            //     table: "PickupRequests");

            // migrationBuilder.DropIndex(
            //     name: "IX_MarketplaceItems_PurchasedByFactoryId",
            //     table: "MarketplaceItems");

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DropColumn(
                name: "PreferredPickupDate",
                table: "PickupRequests");

            migrationBuilder.DropColumn(
                name: "WeightUnit",
                table: "PickupRequests");

            migrationBuilder.DropColumn(
                name: "PurchaseDate",
                table: "MarketplaceItems");

            migrationBuilder.DropColumn(
                name: "PurchasePrice",
                table: "MarketplaceItems");

            migrationBuilder.DropColumn(
                name: "StripePaymentIntentId",
                table: "MarketplaceItems");

            migrationBuilder.DropColumn(
                name: "WeightUnit",
                table: "MarketplaceItems");

            migrationBuilder.RenameColumn(
                name: "Weight",
                table: "PickupRequests",
                newName: "Quantity");

            migrationBuilder.RenameColumn(
                name: "Price",
                table: "PickupRequests",
                newName: "ProposedPricePerUnit");

            migrationBuilder.RenameColumn(
                name: "ApprovedByAdminId",
                table: "PickupRequests",
                newName: "Unit");

            migrationBuilder.RenameColumn(
                name: "Weight",
                table: "MarketplaceItems",
                newName: "Quantity");

            migrationBuilder.RenameColumn(
                name: "Status",
                table: "MarketplaceItems",
                newName: "UserId");

            migrationBuilder.RenameColumn(
                name: "PurchasedByFactoryId",
                table: "MarketplaceItems",
                newName: "Unit");

            migrationBuilder.RenameColumn(
                name: "PublishedDate",
                table: "MarketplaceItems",
                newName: "UpdatedAt");

            migrationBuilder.RenameColumn(
                name: "Price",
                table: "MarketplaceItems",
                newName: "PricePerUnit");

            migrationBuilder.AlterColumn<int>(
                name: "MaterialType",
                table: "PickupRequests",
                type: "int",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(100)",
                oldMaxLength: 100)
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "PickupRequests",
                type: "char(36)",
                nullable: false,
                collation: "ascii_general_ci",
                oldClrType: typeof(int),
                oldType: "int")
                .OldAnnotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn);

            migrationBuilder.AddColumn<int>(
                name: "AdminId",
                table: "PickupRequests",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "PickupRequests",
                type: "datetime(6)",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "ImageUrls",
                table: "PickupRequests",
                type: "longtext",
                nullable: false)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAt",
                table: "PickupRequests",
                type: "datetime(6)",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AlterColumn<Guid>(
                name: "PickupRequestId",
                table: "MarketplaceItems",
                type: "char(36)",
                nullable: false,
                collation: "ascii_general_ci",
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<int>(
                name: "MaterialType",
                table: "MarketplaceItems",
                type: "int",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(100)",
                oldMaxLength: 100)
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "MarketplaceItems",
                type: "char(36)",
                nullable: false,
                collation: "ascii_general_ci",
                oldClrType: typeof(int),
                oldType: "int")
                .OldAnnotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "MarketplaceItems",
                type: "datetime(6)",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<int>(
                name: "FactoryId",
                table: "MarketplaceItems",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ImageUrls",
                table: "MarketplaceItems",
                type: "longtext",
                nullable: false)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<bool>(
                name: "IsAvailable",
                table: "MarketplaceItems",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "PublishedAt",
                table: "MarketplaceItems",
                type: "datetime(6)",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.CreateTable(
                name: "Notifications",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    Title = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Message = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    IsRead = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    NotificationType = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Notifications", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Notifications_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Purchases",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    MarketplaceItemId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    FactoryId = table.Column<int>(type: "int", nullable: false),
                    Quantity = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    PricePerUnit = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    StripePaymentIntentId = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    PaymentStatus = table.Column<int>(type: "int", nullable: false),
                    PurchaseDate = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Purchases", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Purchases_Factories_FactoryId",
                        column: x => x.FactoryId,
                        principalTable: "Factories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Purchases_MarketplaceItems_MarketplaceItemId",
                        column: x => x.MarketplaceItemId,
                        principalTable: "MarketplaceItems",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_PickupRequests_AdminId",
                table: "PickupRequests",
                column: "AdminId");

            migrationBuilder.CreateIndex(
                name: "IX_MarketplaceItems_FactoryId",
                table: "MarketplaceItems",
                column: "FactoryId");

            migrationBuilder.CreateIndex(
                name: "IX_MarketplaceItems_UserId",
                table: "MarketplaceItems",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Notifications_UserId",
                table: "Notifications",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Purchases_FactoryId",
                table: "Purchases",
                column: "FactoryId");

            migrationBuilder.CreateIndex(
                name: "IX_Purchases_MarketplaceItemId",
                table: "Purchases",
                column: "MarketplaceItemId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_MarketplaceItems_Factories_FactoryId",
                table: "MarketplaceItems",
                column: "FactoryId",
                principalTable: "Factories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_MarketplaceItems_Users_UserId",
                table: "MarketplaceItems",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_PickupRequests_Users_AdminId",
                table: "PickupRequests",
                column: "AdminId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MarketplaceItems_Factories_FactoryId",
                table: "MarketplaceItems");

            migrationBuilder.DropForeignKey(
                name: "FK_MarketplaceItems_Users_UserId",
                table: "MarketplaceItems");

            migrationBuilder.DropForeignKey(
                name: "FK_PickupRequests_Users_AdminId",
                table: "PickupRequests");

            migrationBuilder.DropTable(
                name: "Notifications");

            migrationBuilder.DropTable(
                name: "Purchases");

            migrationBuilder.DropIndex(
                name: "IX_PickupRequests_AdminId",
                table: "PickupRequests");

            migrationBuilder.DropIndex(
                name: "IX_MarketplaceItems_FactoryId",
                table: "MarketplaceItems");

            migrationBuilder.DropIndex(
                name: "IX_MarketplaceItems_UserId",
                table: "MarketplaceItems");

            migrationBuilder.DropColumn(
                name: "AdminId",
                table: "PickupRequests");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "PickupRequests");

            migrationBuilder.DropColumn(
                name: "ImageUrls",
                table: "PickupRequests");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "PickupRequests");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "MarketplaceItems");

            migrationBuilder.DropColumn(
                name: "FactoryId",
                table: "MarketplaceItems");

            migrationBuilder.DropColumn(
                name: "ImageUrls",
                table: "MarketplaceItems");

            migrationBuilder.DropColumn(
                name: "IsAvailable",
                table: "MarketplaceItems");

            migrationBuilder.DropColumn(
                name: "PublishedAt",
                table: "MarketplaceItems");

            migrationBuilder.RenameColumn(
                name: "Unit",
                table: "PickupRequests",
                newName: "ApprovedByAdminId");

            migrationBuilder.RenameColumn(
                name: "Quantity",
                table: "PickupRequests",
                newName: "Weight");

            migrationBuilder.RenameColumn(
                name: "ProposedPricePerUnit",
                table: "PickupRequests",
                newName: "Price");

            migrationBuilder.RenameColumn(
                name: "UserId",
                table: "MarketplaceItems",
                newName: "Status");

            migrationBuilder.RenameColumn(
                name: "UpdatedAt",
                table: "MarketplaceItems",
                newName: "PublishedDate");

            migrationBuilder.RenameColumn(
                name: "Unit",
                table: "MarketplaceItems",
                newName: "PurchasedByFactoryId");

            migrationBuilder.RenameColumn(
                name: "Quantity",
                table: "MarketplaceItems",
                newName: "Weight");

            migrationBuilder.RenameColumn(
                name: "PricePerUnit",
                table: "MarketplaceItems",
                newName: "Price");

            migrationBuilder.AlterColumn<string>(
                name: "MaterialType",
                table: "PickupRequests",
                type: "varchar(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int")
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<int>(
                name: "Id",
                table: "PickupRequests",
                type: "int",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "char(36)")
                .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn)
                .OldAnnotation("Relational:Collation", "ascii_general_ci");

            migrationBuilder.AddColumn<DateTime>(
                name: "PreferredPickupDate",
                table: "PickupRequests",
                type: "datetime(6)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "WeightUnit",
                table: "PickupRequests",
                type: "varchar(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "")
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<int>(
                name: "PickupRequestId",
                table: "MarketplaceItems",
                type: "int",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "char(36)")
                .OldAnnotation("Relational:Collation", "ascii_general_ci");

            migrationBuilder.AlterColumn<string>(
                name: "MaterialType",
                table: "MarketplaceItems",
                type: "varchar(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int")
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<int>(
                name: "Id",
                table: "MarketplaceItems",
                type: "int",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "char(36)")
                .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn)
                .OldAnnotation("Relational:Collation", "ascii_general_ci");

            migrationBuilder.AddColumn<DateTime>(
                name: "PurchaseDate",
                table: "MarketplaceItems",
                type: "datetime(6)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "PurchasePrice",
                table: "MarketplaceItems",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "StripePaymentIntentId",
                table: "MarketplaceItems",
                type: "varchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "")
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "WeightUnit",
                table: "MarketplaceItems",
                type: "varchar(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "")
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "Address", "Age", "CreatedAt", "Email", "IsActive", "Name", "PasswordHash", "Role" },
                values: new object[] { 1, "Admin Address", 30, new DateTime(2025, 9, 1, 7, 56, 6, 562, DateTimeKind.Utc).AddTicks(9801), "admin@nadafa.com", true, "Admin User", "$2a$11$6yT2gFxRR5Kti4.5z5c1fecvhB0rWvq2foD/DslmBxPqEdnFn4/G6", 2 });

            migrationBuilder.CreateIndex(
                name: "IX_PickupRequests_ApprovedByAdminId",
                table: "PickupRequests",
                column: "ApprovedByAdminId");

            migrationBuilder.CreateIndex(
                name: "IX_MarketplaceItems_PurchasedByFactoryId",
                table: "MarketplaceItems",
                column: "PurchasedByFactoryId");

            migrationBuilder.AddForeignKey(
                name: "FK_MarketplaceItems_Factories_PurchasedByFactoryId",
                table: "MarketplaceItems",
                column: "PurchasedByFactoryId",
                principalTable: "Factories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_PickupRequests_Users_ApprovedByAdminId",
                table: "PickupRequests",
                column: "ApprovedByAdminId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
