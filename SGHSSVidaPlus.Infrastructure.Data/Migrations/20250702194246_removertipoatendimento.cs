using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SGHSSVidaPlus.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class removertipoatendimento : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AgendamentosTiposAtendimento");

            migrationBuilder.DropTable(
                name: "TiposAtendimento");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "TiposAtendimento",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Ativo = table.Column<bool>(type: "bit", nullable: false),
                    DataInclusao = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Nome = table.Column<string>(type: "varchar(200)", nullable: false),
                    UsuarioInclusao = table.Column<string>(type: "varchar(30)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TiposAtendimento", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AgendamentosTiposAtendimento",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AgendamentoId = table.Column<int>(type: "int", nullable: false),
                    TipoAtendimentoId = table.Column<int>(type: "int", nullable: false)
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
                name: "IX_AgendamentosTiposAtendimento_AgendamentoId",
                table: "AgendamentosTiposAtendimento",
                column: "AgendamentoId");

            migrationBuilder.CreateIndex(
                name: "IX_AgendamentosTiposAtendimento_TipoAtendimentoId",
                table: "AgendamentosTiposAtendimento",
                column: "TipoAtendimentoId");
        }
    }
}
