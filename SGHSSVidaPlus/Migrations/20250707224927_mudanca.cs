using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SGHSSVidaPlus.MVC.Migrations
{
    /// <inheritdoc />
    public partial class mudanca : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "a2dfa1e2-b1d5-4a8e-a9b0-a3e7e0e7a1e2",
                column: "ConcurrencyStamp",
                value: "fb977d3c-09a5-46a1-8d51-5b6c138128dc");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "b743329b-2839-4d64-968b-f417b7b9f847",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "52de708a-487f-413c-b83a-285bcbf64e9e", "AQAAAAIAAYagAAAAEAfq+geUgbWbn5rDJkMMnlESZ4K2uxu45Jiqec1kPsMbzxa1zDa/5AAZ3La7eO+86A==", "c520f9b2-3ec0-4d5c-896c-7f9a5e6404d0" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "a2dfa1e2-b1d5-4a8e-a9b0-a3e7e0e7a1e2",
                column: "ConcurrencyStamp",
                value: "c6cf94d1-d090-4c7a-a009-a28e8cf0d202");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "b743329b-2839-4d64-968b-f417b7b9f847",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "ba1e9cae-22cc-4858-9234-dd88a91730b2", "AQAAAAIAAYagAAAAECX0yTjugC1pJLQeYA88aT6AIK8uDTWkQViVFzaofkq7ACJPxUBCyDwjpadFIWtp+w==", "b3cef250-508b-46ca-afe0-922f995144ee" });
        }
    }
}
