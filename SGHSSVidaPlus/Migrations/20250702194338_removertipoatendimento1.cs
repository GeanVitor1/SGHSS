using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SGHSSVidaPlus.MVC.Migrations
{
    /// <inheritdoc />
    public partial class removertipoatendimento1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "a2dfa1e2-b1d5-4a8e-a9b0-a3e7e0e7a1e2",
                column: "ConcurrencyStamp",
                value: "cc227245-926e-4f7b-bf82-19da6aef8126");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "b743329b-2839-4d64-968b-f417b7b9f847",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "84a0b66f-f361-4358-8f32-46eb8ffa7ecb", "AQAAAAIAAYagAAAAEPhW257t2XPbN1tslDAnDe5MOeExR15FzascR7sLjbukiY41AJvmNsNWvJaGJpwskQ==", "f2647b04-292c-4e7d-8365-2a6531cd4349" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
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
    }
}
