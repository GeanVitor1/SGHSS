using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SGHSSVidaPlus.MVC.Migrations
{
    /// <inheritdoc />
    public partial class teste : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "a2dfa1e2-b1d5-4a8e-a9b0-a3e7e0e7a1e2",
                column: "ConcurrencyStamp",
                value: "1b9cddfc-2f0e-49f6-a5b0-2722ec82806a");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "b743329b-2839-4d64-968b-f417b7b9f847",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "9bfff518-58c2-4767-8eb0-f73c01209a1b", "AQAAAAIAAYagAAAAEP+G3mDkMMVJkkj5VlxV8cPyM0lNoJsf+tdwN9auCkV+9DOKZzuBUPBsL1tmSxjJ9w==", "1b2f2117-43d9-4ae1-b89a-983bcf786105" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "a2dfa1e2-b1d5-4a8e-a9b0-a3e7e0e7a1e2",
                column: "ConcurrencyStamp",
                value: "14173c6f-f402-46ca-80e6-fd0609a8cf97");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "b743329b-2839-4d64-968b-f417b7b9f847",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "206ce6e4-3e5a-486b-8c48-4b51aaff0a6b", "AQAAAAIAAYagAAAAEFoW7bcKT1r7R4sJpN7vCF8ANJZHUXeYcitqB4bkDU6npgWMBo9msg3Tk+JfKiiDUA==", "7a0be62c-a414-4d65-94d1-a5ab372a3a14" });
        }
    }
}
