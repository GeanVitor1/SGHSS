using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SGHSSVidaPlus.MVC.Migrations
{
    /// <inheritdoc />
    public partial class AddDefaultAtendenteProfessional : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
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
    }
}
