using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SGHSSVidaPlus.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddDefaultAtendenteProfessional : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "ProfissionalSaude",
                columns: new[] { "Id", "Ativo", "Cargo", "DataInclusao", "Email", "EspecialidadeCargo", "Nome", "RegistroConselho", "Telefone", "UsuarioInclusao" },
                values: new object[] { 1, true, "Atendente", new DateTime(2023, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "atendente.padrao@sghssvidaplus.com.br", "Geral", "Atendente de Agendamentos Pendentes", "N/A", "(XX) XXXX-XXXX", "Sistema" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "ProfissionalSaude",
                keyColumn: "Id",
                keyValue: 1);
        }
    }
}
