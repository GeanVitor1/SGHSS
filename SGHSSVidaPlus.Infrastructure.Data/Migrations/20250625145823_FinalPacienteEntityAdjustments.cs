using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SGHSSVidaPlus.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class FinalPacienteEntityAdjustments : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsWhatsApp",
                table: "PacienteContatos",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AlterColumn<string>(
                name: "ProfissionalResponsavel",
                table: "HistoricoPacientes",
                type: "varchar(100)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "varchar(100)");

            migrationBuilder.AlterColumn<string>(
                name: "Descricao",
                table: "HistoricoPacientes",
                type: "varchar(4000)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "varchar(4000)");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsWhatsApp",
                table: "PacienteContatos");

            migrationBuilder.AlterColumn<string>(
                name: "ProfissionalResponsavel",
                table: "HistoricoPacientes",
                type: "varchar(100)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "varchar(100)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Descricao",
                table: "HistoricoPacientes",
                type: "varchar(4000)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "varchar(4000)",
                oldNullable: true);
        }
    }
}
