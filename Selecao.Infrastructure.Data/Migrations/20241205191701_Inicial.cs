using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Selecao.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class Inicial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Candidatos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nome = table.Column<string>(type: "varchar(200)", nullable: false),
                    DataNascimento = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Logradouro = table.Column<string>(type: "varchar(300)", nullable: false),
                    Bairro = table.Column<string>(type: "varchar(50)", nullable: false),
                    EstadoCivil = table.Column<string>(type: "varchar(100)", nullable: false),
                    Selecionado = table.Column<bool>(type: "bit", nullable: false),
                    Banido = table.Column<bool>(type: "bit", nullable: false),
                    UsuarioInclusao = table.Column<string>(type: "varchar(30)", nullable: false),
                    DataInclusao = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Candidatos", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "EtapasSelecao",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nome = table.Column<string>(type: "varchar(200)", nullable: false),
                    Status = table.Column<bool>(type: "bit", nullable: false),
                    UsuarioInclusao = table.Column<string>(type: "varchar(30)", nullable: false),
                    DataInclusao = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EtapasSelecao", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Selecoes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Cargo = table.Column<string>(type: "varchar(200)", nullable: false),
                    Vagas = table.Column<int>(type: "int", nullable: false),
                    Objetivo = table.Column<string>(type: "varchar(3000)", nullable: false),
                    UsuarioInclusao = table.Column<string>(type: "varchar(30)", nullable: false),
                    Encerrado = table.Column<bool>(type: "bit", nullable: false),
                    DataInclusao = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UsuarioEncerramento = table.Column<string>(type: "varchar(30)", nullable: false),
                    DataEncerramento = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Selecoes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CandidatoCursos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Titulo = table.Column<string>(type: "varchar(100)", nullable: false),
                    InstituicaoEnsino = table.Column<string>(type: "varchar(100)", nullable: false),
                    DuracaoHoras = table.Column<double>(type: "float", nullable: false),
                    AnoConclusao = table.Column<string>(type: "varchar(100)", nullable: false),
                    Descricao = table.Column<string>(type: "varchar(100)", nullable: false),
                    Area = table.Column<string>(type: "varchar(100)", nullable: false),
                    CandidatoId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CandidatoCursos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CandidatoCursos_Candidatos_CandidatoId",
                        column: x => x.CandidatoId,
                        principalTable: "Candidatos",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "CandidatoExperiencias",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Cargo = table.Column<string>(type: "varchar(100)", nullable: false),
                    Empregador = table.Column<string>(type: "varchar(100)", nullable: false),
                    Inicio = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Termino = table.Column<DateTime>(type: "datetime2", nullable: true),
                    TrabalhoAtual = table.Column<bool>(type: "bit", nullable: false),
                    Duracao = table.Column<string>(type: "varchar(100)", nullable: false),
                    Descricao = table.Column<string>(type: "varchar(100)", nullable: false),
                    IsInformatica = table.Column<bool>(type: "bit", nullable: false),
                    CandidatoId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CandidatoExperiencias", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CandidatoExperiencias_Candidatos_CandidatoId",
                        column: x => x.CandidatoId,
                        principalTable: "Candidatos",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "CandidatoFormacoes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Titulo = table.Column<string>(type: "varchar(100)", nullable: false),
                    Area = table.Column<string>(type: "varchar(100)", nullable: false),
                    InstituicaoEnsino = table.Column<string>(type: "varchar(100)", nullable: false),
                    AnoConclusao = table.Column<string>(type: "varchar(100)", nullable: false),
                    Descricao = table.Column<string>(type: "varchar(100)", nullable: false),
                    CandidatoId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CandidatoFormacoes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CandidatoFormacoes_Candidatos_CandidatoId",
                        column: x => x.CandidatoId,
                        principalTable: "Candidatos",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "CandidatosContatos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Contato = table.Column<string>(type: "varchar(200)", nullable: false),
                    Tipo = table.Column<string>(type: "varchar(100)", nullable: false),
                    IsWhatsApp = table.Column<bool>(type: "bit", nullable: false),
                    CandidatoId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CandidatosContatos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CandidatosContatos_Candidatos_CandidatoId",
                        column: x => x.CandidatoId,
                        principalTable: "Candidatos",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "CandidatoSelecao",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CandidatoId = table.Column<int>(type: "int", nullable: false),
                    SelecaoId = table.Column<int>(type: "int", nullable: false),
                    Selecionado = table.Column<bool>(type: "bit", nullable: false),
                    Eliminado = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CandidatoSelecao", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CandidatoSelecao_Candidatos_CandidatoId",
                        column: x => x.CandidatoId,
                        principalTable: "Candidatos",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_CandidatoSelecao_Selecoes_SelecaoId",
                        column: x => x.SelecaoId,
                        principalTable: "Selecoes",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "EtapaSelecao",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EtapaId = table.Column<int>(type: "int", nullable: false),
                    SelecaoId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EtapaSelecao", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EtapaSelecao_EtapasSelecao_EtapaId",
                        column: x => x.EtapaId,
                        principalTable: "EtapasSelecao",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_EtapaSelecao_Selecoes_SelecaoId",
                        column: x => x.SelecaoId,
                        principalTable: "Selecoes",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_CandidatoCursos_CandidatoId",
                table: "CandidatoCursos",
                column: "CandidatoId");

            migrationBuilder.CreateIndex(
                name: "IX_CandidatoExperiencias_CandidatoId",
                table: "CandidatoExperiencias",
                column: "CandidatoId");

            migrationBuilder.CreateIndex(
                name: "IX_CandidatoFormacoes_CandidatoId",
                table: "CandidatoFormacoes",
                column: "CandidatoId");

            migrationBuilder.CreateIndex(
                name: "IX_CandidatosContatos_CandidatoId",
                table: "CandidatosContatos",
                column: "CandidatoId");

            migrationBuilder.CreateIndex(
                name: "IX_CandidatoSelecao_CandidatoId",
                table: "CandidatoSelecao",
                column: "CandidatoId");

            migrationBuilder.CreateIndex(
                name: "IX_CandidatoSelecao_SelecaoId",
                table: "CandidatoSelecao",
                column: "SelecaoId");

            migrationBuilder.CreateIndex(
                name: "IX_EtapaSelecao_EtapaId",
                table: "EtapaSelecao",
                column: "EtapaId");

            migrationBuilder.CreateIndex(
                name: "IX_EtapaSelecao_SelecaoId",
                table: "EtapaSelecao",
                column: "SelecaoId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CandidatoCursos");

            migrationBuilder.DropTable(
                name: "CandidatoExperiencias");

            migrationBuilder.DropTable(
                name: "CandidatoFormacoes");

            migrationBuilder.DropTable(
                name: "CandidatosContatos");

            migrationBuilder.DropTable(
                name: "CandidatoSelecao");

            migrationBuilder.DropTable(
                name: "EtapaSelecao");

            migrationBuilder.DropTable(
                name: "Candidatos");

            migrationBuilder.DropTable(
                name: "EtapasSelecao");

            migrationBuilder.DropTable(
                name: "Selecoes");
        }
    }
}
