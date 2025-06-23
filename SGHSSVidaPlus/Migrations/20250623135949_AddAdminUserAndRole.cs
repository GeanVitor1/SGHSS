using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SGHSSVidaPlus.MVC.Migrations
{
    /// <inheritdoc />
    public partial class AddAdminUserAndRole : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[] { "a2dfa1e2-b1d5-4a8e-a9b0-a3e7e0e7a1e2", "12acab4d-7a9e-4ae1-bf8c-47c32c1de52d", "admin", "ADMIN" });

            migrationBuilder.InsertData(
                table: "AspNetUsers",
                columns: new[] { "Id", "AccessFailedCount", "Admin", "Bloqueado", "ConcurrencyStamp", "Email", "EmailConfirmed", "LockoutEnabled", "LockoutEnd", "Nome", "NormalizedEmail", "NormalizedUserName", "PasswordHash", "PhoneNumber", "PhoneNumberConfirmed", "SecurityStamp", "TwoFactorEnabled", "UserName" },
                values: new object[] { "b743329b-2839-4d64-968b-f417b7b9f847", 0, true, false, "ec71adfa-a8fc-4a89-88b9-f9834f48c2c2", "admin@sghssvidaplus.com.br", true, false, null, "Administrador Master", "ADMIN@SGHSSVIDAPLUS.COM.BR", "ADMIN", "AQAAAAIAAYagAAAAEBx5yp9MhitZq0APnXItAhrCJjcaT2+fntJP1tmXPR4Gqr6nFZYzW8Dbug0o47J45Q==", null, false, "9501b1f5-c77a-4f8c-b841-f2b9c530bf05", false, "admin" });

            migrationBuilder.InsertData(
                table: "AspNetUserRoles",
                columns: new[] { "RoleId", "UserId" },
                values: new object[] { "a2dfa1e2-b1d5-4a8e-a9b0-a3e7e0e7a1e2", "b743329b-2839-4d64-968b-f417b7b9f847" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetUserRoles",
                keyColumns: new[] { "RoleId", "UserId" },
                keyValues: new object[] { "a2dfa1e2-b1d5-4a8e-a9b0-a3e7e0e7a1e2", "b743329b-2839-4d64-968b-f417b7b9f847" });

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "a2dfa1e2-b1d5-4a8e-a9b0-a3e7e0e7a1e2");

            migrationBuilder.DeleteData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "b743329b-2839-4d64-968b-f417b7b9f847");
        }
    }
}
