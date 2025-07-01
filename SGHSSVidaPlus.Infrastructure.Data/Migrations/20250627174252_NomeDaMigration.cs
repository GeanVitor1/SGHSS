using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SGHSSVidaPlus.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class NomeDaMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Agendamentos_ProfissionaisSaude_ProfissionalResponsavelId",
                table: "Agendamentos");

            migrationBuilder.DropForeignKey(
                name: "FK_CursosCertificacoesProfissionaisSaude_ProfissionaisSaude_ProfissionalSaudeId",
                table: "CursosCertificacoesProfissionaisSaude");

            migrationBuilder.DropForeignKey(
                name: "FK_FormacoesAcademicasProfissionaisSaude_ProfissionaisSaude_ProfissionalSaudeId",
                table: "FormacoesAcademicasProfissionaisSaude");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ProfissionaisSaude",
                table: "ProfissionaisSaude");

            migrationBuilder.DropPrimaryKey(
                name: "PK_FormacoesAcademicasProfissionaisSaude",
                table: "FormacoesAcademicasProfissionaisSaude");

            migrationBuilder.DropPrimaryKey(
                name: "PK_CursosCertificacoesProfissionaisSaude",
                table: "CursosCertificacoesProfissionaisSaude");

            migrationBuilder.RenameTable(
                name: "ProfissionaisSaude",
                newName: "ProfissionalSaude");

            migrationBuilder.RenameTable(
                name: "FormacoesAcademicasProfissionaisSaude",
                newName: "FormacoesAcademicasProfissionalSaude");

            migrationBuilder.RenameTable(
                name: "CursosCertificacoesProfissionaisSaude",
                newName: "CursosCertificacoesProfissionalSaude");

            migrationBuilder.RenameIndex(
                name: "IX_FormacoesAcademicasProfissionaisSaude_ProfissionalSaudeId",
                table: "FormacoesAcademicasProfissionalSaude",
                newName: "IX_FormacoesAcademicasProfissionalSaude_ProfissionalSaudeId");

            migrationBuilder.RenameIndex(
                name: "IX_CursosCertificacoesProfissionaisSaude_ProfissionalSaudeId",
                table: "CursosCertificacoesProfissionalSaude",
                newName: "IX_CursosCertificacoesProfissionalSaude_ProfissionalSaudeId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ProfissionalSaude",
                table: "ProfissionalSaude",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_FormacoesAcademicasProfissionalSaude",
                table: "FormacoesAcademicasProfissionalSaude",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_CursosCertificacoesProfissionalSaude",
                table: "CursosCertificacoesProfissionalSaude",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Agendamentos_ProfissionalSaude_ProfissionalResponsavelId",
                table: "Agendamentos",
                column: "ProfissionalResponsavelId",
                principalTable: "ProfissionalSaude",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_CursosCertificacoesProfissionalSaude_ProfissionalSaude_ProfissionalSaudeId",
                table: "CursosCertificacoesProfissionalSaude",
                column: "ProfissionalSaudeId",
                principalTable: "ProfissionalSaude",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_FormacoesAcademicasProfissionalSaude_ProfissionalSaude_ProfissionalSaudeId",
                table: "FormacoesAcademicasProfissionalSaude",
                column: "ProfissionalSaudeId",
                principalTable: "ProfissionalSaude",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Agendamentos_ProfissionalSaude_ProfissionalResponsavelId",
                table: "Agendamentos");

            migrationBuilder.DropForeignKey(
                name: "FK_CursosCertificacoesProfissionalSaude_ProfissionalSaude_ProfissionalSaudeId",
                table: "CursosCertificacoesProfissionalSaude");

            migrationBuilder.DropForeignKey(
                name: "FK_FormacoesAcademicasProfissionalSaude_ProfissionalSaude_ProfissionalSaudeId",
                table: "FormacoesAcademicasProfissionalSaude");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ProfissionalSaude",
                table: "ProfissionalSaude");

            migrationBuilder.DropPrimaryKey(
                name: "PK_FormacoesAcademicasProfissionalSaude",
                table: "FormacoesAcademicasProfissionalSaude");

            migrationBuilder.DropPrimaryKey(
                name: "PK_CursosCertificacoesProfissionalSaude",
                table: "CursosCertificacoesProfissionalSaude");

            migrationBuilder.RenameTable(
                name: "ProfissionalSaude",
                newName: "ProfissionaisSaude");

            migrationBuilder.RenameTable(
                name: "FormacoesAcademicasProfissionalSaude",
                newName: "FormacoesAcademicasProfissionaisSaude");

            migrationBuilder.RenameTable(
                name: "CursosCertificacoesProfissionalSaude",
                newName: "CursosCertificacoesProfissionaisSaude");

            migrationBuilder.RenameIndex(
                name: "IX_FormacoesAcademicasProfissionalSaude_ProfissionalSaudeId",
                table: "FormacoesAcademicasProfissionaisSaude",
                newName: "IX_FormacoesAcademicasProfissionaisSaude_ProfissionalSaudeId");

            migrationBuilder.RenameIndex(
                name: "IX_CursosCertificacoesProfissionalSaude_ProfissionalSaudeId",
                table: "CursosCertificacoesProfissionaisSaude",
                newName: "IX_CursosCertificacoesProfissionaisSaude_ProfissionalSaudeId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ProfissionaisSaude",
                table: "ProfissionaisSaude",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_FormacoesAcademicasProfissionaisSaude",
                table: "FormacoesAcademicasProfissionaisSaude",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_CursosCertificacoesProfissionaisSaude",
                table: "CursosCertificacoesProfissionaisSaude",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Agendamentos_ProfissionaisSaude_ProfissionalResponsavelId",
                table: "Agendamentos",
                column: "ProfissionalResponsavelId",
                principalTable: "ProfissionaisSaude",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_CursosCertificacoesProfissionaisSaude_ProfissionaisSaude_ProfissionalSaudeId",
                table: "CursosCertificacoesProfissionaisSaude",
                column: "ProfissionalSaudeId",
                principalTable: "ProfissionaisSaude",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_FormacoesAcademicasProfissionaisSaude_ProfissionaisSaude_ProfissionalSaudeId",
                table: "FormacoesAcademicasProfissionaisSaude",
                column: "ProfissionalSaudeId",
                principalTable: "ProfissionaisSaude",
                principalColumn: "Id");
        }
    }
}
