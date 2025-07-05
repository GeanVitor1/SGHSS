using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SGHSSVidaPlus.MVC.Migrations
{
    /// <inheritdoc />
    public partial class cadastropaciente : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
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
    }
}
