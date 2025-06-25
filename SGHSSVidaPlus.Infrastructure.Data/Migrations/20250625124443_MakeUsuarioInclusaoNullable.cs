using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SGHSSVidaPlus.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class MakeUsuarioInclusaoNullable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "UsuarioInclusao",
                table: "Pacientes",
                type: "varchar(30)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "varchar(30)");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "UsuarioInclusao",
                table: "Pacientes",
                type: "varchar(30)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "varchar(30)",
                oldNullable: true);
        }
    }
}
