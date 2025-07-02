using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SGHSSVidaPlus.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class initialdatabase : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Pacientes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nome = table.Column<string>(type: "varchar(200)", nullable: false),
                    DataNascimento = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CPF = table.Column<string>(type: "varchar(14)", nullable: false),
                    Endereco = table.Column<string>(type: "varchar(500)", nullable: false),
                    EstadoCivil = table.Column<string>(type: "varchar(20)", nullable: false),
                    Ativo = table.Column<bool>(type: "bit", nullable: false),
                    UsuarioInclusao = table.Column<string>(type: "varchar(30)", nullable: true),
                    DataInclusao = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Pacientes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ProfissionalSaude",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nome = table.Column<string>(type: "varchar(200)", nullable: false),
                    Cargo = table.Column<string>(type: "varchar(100)", nullable: false),
                    EspecialidadeCargo = table.Column<string>(type: "varchar(100)", nullable: true),
                    Telefone = table.Column<string>(type: "varchar(20)", nullable: true),
                    Email = table.Column<string>(type: "varchar(100)", nullable: true),
                    RegistroConselho = table.Column<string>(type: "varchar(50)", nullable: true),
                    Ativo = table.Column<bool>(type: "bit", nullable: false),
                    UsuarioInclusao = table.Column<string>(type: "varchar(30)", nullable: true),
                    DataInclusao = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProfissionalSaude", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TiposAtendimento",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nome = table.Column<string>(type: "varchar(200)", nullable: false),
                    Ativo = table.Column<bool>(type: "bit", nullable: false),
                    UsuarioInclusao = table.Column<string>(type: "varchar(30)", nullable: false),
                    DataInclusao = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TiposAtendimento", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "HistoricoPacientes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Titulo = table.Column<string>(type: "varchar(200)", nullable: false),
                    Descricao = table.Column<string>(type: "varchar(4000)", nullable: true),
                    DataEvento = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ProfissionalResponsavel = table.Column<string>(type: "varchar(100)", nullable: true),
                    PacienteId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HistoricoPacientes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_HistoricoPacientes_Pacientes_PacienteId",
                        column: x => x.PacienteId,
                        principalTable: "Pacientes",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "PacienteContatos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Contato = table.Column<string>(type: "varchar(200)", nullable: false),
                    Tipo = table.Column<string>(type: "varchar(50)", nullable: false),
                    IsWhatsApp = table.Column<bool>(type: "bit", nullable: false),
                    PacienteId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PacienteContatos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PacienteContatos_Pacientes_PacienteId",
                        column: x => x.PacienteId,
                        principalTable: "Pacientes",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Agendamentos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Descricao = table.Column<string>(type: "varchar(300)", nullable: false),
                    Observacoes = table.Column<string>(type: "varchar(100)", nullable: true),
                    Local = table.Column<string>(type: "varchar(100)", nullable: true),
                    DataHoraAgendamento = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Encerrado = table.Column<bool>(type: "bit", nullable: false),
                    UsuarioInclusao = table.Column<string>(type: "varchar(30)", nullable: false),
                    DataInclusao = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UsuarioEncerramento = table.Column<string>(type: "varchar(30)", nullable: true),
                    DataEncerramento = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Status = table.Column<string>(type: "varchar(100)", nullable: false),
                    ProfissionalResponsavelId = table.Column<int>(type: "int", nullable: false),
                    PacienteId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Agendamentos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Agendamentos_Pacientes_PacienteId",
                        column: x => x.PacienteId,
                        principalTable: "Pacientes",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Agendamentos_ProfissionalSaude_ProfissionalResponsavelId",
                        column: x => x.ProfissionalResponsavelId,
                        principalTable: "ProfissionalSaude",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "CursosCertificacoesProfissionalSaude",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Titulo = table.Column<string>(type: "varchar(500)", nullable: false),
                    InstituicaoEnsino = table.Column<string>(type: "varchar(300)", nullable: false),
                    DuracaoHoras = table.Column<double>(type: "float", nullable: false),
                    AnoConclusao = table.Column<string>(type: "varchar(6)", nullable: false),
                    Descricao = table.Column<string>(type: "varchar(2000)", nullable: false),
                    ProfissionalSaudeId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CursosCertificacoesProfissionalSaude", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CursosCertificacoesProfissionalSaude_ProfissionalSaude_ProfissionalSaudeId",
                        column: x => x.ProfissionalSaudeId,
                        principalTable: "ProfissionalSaude",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "FormacoesAcademicasProfissionalSaude",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Titulo = table.Column<string>(type: "varchar(500)", nullable: false),
                    Area = table.Column<string>(type: "varchar(100)", nullable: false),
                    InstituicaoEnsino = table.Column<string>(type: "varchar(300)", nullable: false),
                    AnoConclusao = table.Column<string>(type: "varchar(6)", nullable: false),
                    Descricao = table.Column<string>(type: "varchar(2000)", nullable: false),
                    ProfissionalSaudeId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FormacoesAcademicasProfissionalSaude", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FormacoesAcademicasProfissionalSaude_ProfissionalSaude_ProfissionalSaudeId",
                        column: x => x.ProfissionalSaudeId,
                        principalTable: "ProfissionalSaude",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "AgendamentosPacientes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AgendamentoId = table.Column<int>(type: "int", nullable: false),
                    PacienteId = table.Column<int>(type: "int", nullable: false),
                    Compareceu = table.Column<bool>(type: "bit", nullable: false),
                    AtendimentoFinalizado = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AgendamentosPacientes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AgendamentosPacientes_Agendamentos_AgendamentoId",
                        column: x => x.AgendamentoId,
                        principalTable: "Agendamentos",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_AgendamentosPacientes_Pacientes_PacienteId",
                        column: x => x.PacienteId,
                        principalTable: "Pacientes",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "AgendamentosTiposAtendimento",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TipoAtendimentoId = table.Column<int>(type: "int", nullable: false),
                    AgendamentoId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AgendamentosTiposAtendimento", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AgendamentosTiposAtendimento_Agendamentos_AgendamentoId",
                        column: x => x.AgendamentoId,
                        principalTable: "Agendamentos",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_AgendamentosTiposAtendimento_TiposAtendimento_TipoAtendimentoId",
                        column: x => x.TipoAtendimentoId,
                        principalTable: "TiposAtendimento",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Agendamentos_PacienteId",
                table: "Agendamentos",
                column: "PacienteId");

            migrationBuilder.CreateIndex(
                name: "IX_Agendamentos_ProfissionalResponsavelId",
                table: "Agendamentos",
                column: "ProfissionalResponsavelId");

            migrationBuilder.CreateIndex(
                name: "IX_AgendamentosPacientes_AgendamentoId",
                table: "AgendamentosPacientes",
                column: "AgendamentoId");

            migrationBuilder.CreateIndex(
                name: "IX_AgendamentosPacientes_PacienteId",
                table: "AgendamentosPacientes",
                column: "PacienteId");

            migrationBuilder.CreateIndex(
                name: "IX_AgendamentosTiposAtendimento_AgendamentoId",
                table: "AgendamentosTiposAtendimento",
                column: "AgendamentoId");

            migrationBuilder.CreateIndex(
                name: "IX_AgendamentosTiposAtendimento_TipoAtendimentoId",
                table: "AgendamentosTiposAtendimento",
                column: "TipoAtendimentoId");

            migrationBuilder.CreateIndex(
                name: "IX_CursosCertificacoesProfissionalSaude_ProfissionalSaudeId",
                table: "CursosCertificacoesProfissionalSaude",
                column: "ProfissionalSaudeId");

            migrationBuilder.CreateIndex(
                name: "IX_FormacoesAcademicasProfissionalSaude_ProfissionalSaudeId",
                table: "FormacoesAcademicasProfissionalSaude",
                column: "ProfissionalSaudeId");

            migrationBuilder.CreateIndex(
                name: "IX_HistoricoPacientes_PacienteId",
                table: "HistoricoPacientes",
                column: "PacienteId");

            migrationBuilder.CreateIndex(
                name: "IX_PacienteContatos_PacienteId",
                table: "PacienteContatos",
                column: "PacienteId");

            migrationBuilder.CreateIndex(
                name: "IX_Pacientes_CPF",
                table: "Pacientes",
                column: "CPF",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AgendamentosPacientes");

            migrationBuilder.DropTable(
                name: "AgendamentosTiposAtendimento");

            migrationBuilder.DropTable(
                name: "CursosCertificacoesProfissionalSaude");

            migrationBuilder.DropTable(
                name: "FormacoesAcademicasProfissionalSaude");

            migrationBuilder.DropTable(
                name: "HistoricoPacientes");

            migrationBuilder.DropTable(
                name: "PacienteContatos");

            migrationBuilder.DropTable(
                name: "Agendamentos");

            migrationBuilder.DropTable(
                name: "TiposAtendimento");

            migrationBuilder.DropTable(
                name: "Pacientes");

            migrationBuilder.DropTable(
                name: "ProfissionalSaude");
        }
    }
}
