using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Infrastructure.Data.Migrations
{
    public partial class first : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedAt", "PasswordHash" },
                values: new object[] { new DateTime(2025, 9, 1, 7, 56, 6, 562, DateTimeKind.Utc).AddTicks(9801), "$2a$11$6yT2gFxRR5Kti4.5z5c1fecvhB0rWvq2foD/DslmBxPqEdnFn4/G6" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedAt", "PasswordHash" },
                values: new object[] { new DateTime(2025, 9, 1, 7, 46, 11, 8, DateTimeKind.Utc).AddTicks(8106), "$2a$11$EPyUerSjS5eDr9HqpoAGc.nkMU8IJF/hvrioFwQTiSscR/HDQts6C" });
        }
    }
}
