using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SGHSSVidaPlus.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddEspecialidadeCargoRegistroConselhoToProfissionalSaude : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "UsuarioInclusao",
                table: "ProfissionaisSaude",
                type: "varchar(30)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "varchar(30)");

            migrationBuilder.AlterColumn<string>(
                name: "Telefone",
                table: "ProfissionaisSaude",
                type: "varchar(20)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "varchar(20)");

            migrationBuilder.AlterColumn<string>(
                name: "Email",
                table: "ProfissionaisSaude",
                type: "varchar(100)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "varchar(100)");

            migrationBuilder.AddColumn<string>(
                name: "EspecialidadeCargo",
                table: "ProfissionaisSaude",
                type: "varchar(100)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RegistroConselho",
                table: "ProfissionaisSaude",
                type: "varchar(50)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EspecialidadeCargo",
                table: "ProfissionaisSaude");

            migrationBuilder.DropColumn(
                name: "RegistroConselho",
                table: "ProfissionaisSaude");

            migrationBuilder.AlterColumn<string>(
                name: "UsuarioInclusao",
                table: "ProfissionaisSaude",
                type: "varchar(30)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "varchar(30)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Telefone",
                table: "ProfissionaisSaude",
                type: "varchar(20)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "varchar(20)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Email",
                table: "ProfissionaisSaude",
                type: "varchar(100)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "varchar(100)",
                oldNullable: true);
        }
    }
}
