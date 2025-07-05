using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SGHSSVidaPlus.MVC.Migrations
{
    /// <inheritdoc />
    public partial class cadastropaciente1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "a2dfa1e2-b1d5-4a8e-a9b0-a3e7e0e7a1e2",
                column: "ConcurrencyStamp",
                value: "6fdaba82-fdf6-49ec-b9d5-0fafb3ec0309");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "b743329b-2839-4d64-968b-f417b7b9f847",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "e92aa85d-bd8c-4550-bb0e-28e06ab0e4c0", "AQAAAAIAAYagAAAAEP0aYGxqNyn4fCofryAA5TBhWi5//9/u2nuqCnyfktQqtho3FawjorKXCnTCY7FiSA==", "40f946f4-bff1-47f1-9901-f3385c18b975" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "a2dfa1e2-b1d5-4a8e-a9b0-a3e7e0e7a1e2",
                column: "ConcurrencyStamp",
                value: "d4cd4957-1a78-45d4-ac55-9e139ec947a0");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "b743329b-2839-4d64-968b-f417b7b9f847",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "f2763da5-9fc2-44d4-8bfc-69f796eb5629", "AQAAAAIAAYagAAAAED3QZ7JdJP6uiaJ3fnIFa2nCyEfQpxjRsWXk+e93YgYOe35rk76AT0bbubt441WE2w==", "76edd325-330b-4e37-bafa-ee30a85133af" });
        }
    }
}
