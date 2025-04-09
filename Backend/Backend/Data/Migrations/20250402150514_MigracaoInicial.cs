using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Backend_v02.Data.Migrations
{
    /// <inheritdoc />
    public partial class MigracaoInicial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Escolas",
                columns: table => new
                {
                    IdEscola = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nome = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Localizacao = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Escolas", x => x.IdEscola);
                });

            migrationBuilder.CreateTable(
                name: "UCs",
                columns: table => new
                {
                    IdDisciplina = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    NomeDisciplina = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TipoDisciplina = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    GrauAcademico = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Tipologia = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Semestre = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UCs", x => x.IdDisciplina);
                });

            migrationBuilder.CreateTable(
                name: "Salas",
                columns: table => new
                {
                    IdSala = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nome = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Lugares = table.Column<int>(type: "int", nullable: false),
                    TipoSala = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Localizacao = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    EscolaFK = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Salas", x => x.IdSala);
                    table.ForeignKey(
                        name: "FK_Salas_Escolas_EscolaFK",
                        column: x => x.EscolaFK,
                        principalTable: "Escolas",
                        principalColumn: "IdEscola",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Utilizadores",
                columns: table => new
                {
                    IdUtilizador = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nome = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Funcao = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CategoriaId = table.Column<int>(type: "int", nullable: true),
                    Categoria = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    EscolaFK = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Utilizadores", x => x.IdUtilizador);
                    table.ForeignKey(
                        name: "FK_Utilizadores_Escolas_EscolaFK",
                        column: x => x.EscolaFK,
                        principalTable: "Escolas",
                        principalColumn: "IdEscola",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Turmas",
                columns: table => new
                {
                    IdTurma = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nome = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DisciplinaFK = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Turmas", x => x.IdTurma);
                    table.ForeignKey(
                        name: "FK_Turmas_UCs_DisciplinaFK",
                        column: x => x.DisciplinaFK,
                        principalTable: "UCs",
                        principalColumn: "IdDisciplina",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "EscolaUtilizadorComissao",
                columns: table => new
                {
                    EscolasOndeEnsinaIdEscola = table.Column<int>(type: "int", nullable: false),
                    MembrosComissaoHorariosIdUtilizador = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EscolaUtilizadorComissao", x => new { x.EscolasOndeEnsinaIdEscola, x.MembrosComissaoHorariosIdUtilizador });
                    table.ForeignKey(
                        name: "FK_EscolaUtilizadorComissao_Escolas_EscolasOndeEnsinaIdEscola",
                        column: x => x.EscolasOndeEnsinaIdEscola,
                        principalTable: "Escolas",
                        principalColumn: "IdEscola",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_EscolaUtilizadorComissao_Utilizadores_MembrosComissaoHorariosIdUtilizador",
                        column: x => x.MembrosComissaoHorariosIdUtilizador,
                        principalTable: "Utilizadores",
                        principalColumn: "IdUtilizador",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UCDocentes",
                columns: table => new
                {
                    DisciplinasLecionadasIdDisciplina = table.Column<int>(type: "int", nullable: false),
                    DocentesIdUtilizador = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UCDocentes", x => new { x.DisciplinasLecionadasIdDisciplina, x.DocentesIdUtilizador });
                    table.ForeignKey(
                        name: "FK_UCDocentes_UCs_DisciplinasLecionadasIdDisciplina",
                        column: x => x.DisciplinasLecionadasIdDisciplina,
                        principalTable: "UCs",
                        principalColumn: "IdDisciplina",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UCDocentes_Utilizadores_DocentesIdUtilizador",
                        column: x => x.DocentesIdUtilizador,
                        principalTable: "Utilizadores",
                        principalColumn: "IdUtilizador",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "BlocosHorario",
                columns: table => new
                {
                    IdBloco = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    HoraInicio = table.Column<TimeSpan>(type: "time", nullable: false),
                    HoraFim = table.Column<TimeSpan>(type: "time", nullable: false),
                    DiaSemana = table.Column<int>(type: "int", nullable: false),
                    ProfessorFK = table.Column<int>(type: "int", nullable: false),
                    DisciplinaFK = table.Column<int>(type: "int", nullable: false),
                    SalaFK = table.Column<int>(type: "int", nullable: false),
                    TurmaFK = table.Column<int>(type: "int", nullable: false),
                    TipologiaFK = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BlocosHorario", x => x.IdBloco);
                    table.ForeignKey(
                        name: "FK_BlocosHorario_Salas_SalaFK",
                        column: x => x.SalaFK,
                        principalTable: "Salas",
                        principalColumn: "IdSala",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_BlocosHorario_Turmas_TurmaFK",
                        column: x => x.TurmaFK,
                        principalTable: "Turmas",
                        principalColumn: "IdTurma",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_BlocosHorario_UCs_DisciplinaFK",
                        column: x => x.DisciplinaFK,
                        principalTable: "UCs",
                        principalColumn: "IdDisciplina",
                        onDelete: ReferentialAction.NoAction);
                    table.ForeignKey(
                        name: "FK_BlocosHorario_UCs_TipologiaFK",
                        column: x => x.TipologiaFK,
                        principalTable: "UCs",
                        principalColumn: "IdDisciplina",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_BlocosHorario_Utilizadores_ProfessorFK",
                        column: x => x.ProfessorFK,
                        principalTable: "Utilizadores",
                        principalColumn: "IdUtilizador",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_BlocosHorario_DisciplinaFK",
                table: "BlocosHorario",
                column: "DisciplinaFK");

            migrationBuilder.CreateIndex(
                name: "IX_BlocosHorario_ProfessorFK",
                table: "BlocosHorario",
                column: "ProfessorFK");

            migrationBuilder.CreateIndex(
                name: "IX_BlocosHorario_SalaFK",
                table: "BlocosHorario",
                column: "SalaFK");

            migrationBuilder.CreateIndex(
                name: "IX_BlocosHorario_TipologiaFK",
                table: "BlocosHorario",
                column: "TipologiaFK");

            migrationBuilder.CreateIndex(
                name: "IX_BlocosHorario_TurmaFK",
                table: "BlocosHorario",
                column: "TurmaFK");

            migrationBuilder.CreateIndex(
                name: "IX_EscolaUtilizadorComissao_MembrosComissaoHorariosIdUtilizador",
                table: "EscolaUtilizadorComissao",
                column: "MembrosComissaoHorariosIdUtilizador");

            migrationBuilder.CreateIndex(
                name: "IX_Salas_EscolaFK",
                table: "Salas",
                column: "EscolaFK");

            migrationBuilder.CreateIndex(
                name: "IX_Turmas_DisciplinaFK",
                table: "Turmas",
                column: "DisciplinaFK");

            migrationBuilder.CreateIndex(
                name: "IX_UCDocentes_DocentesIdUtilizador",
                table: "UCDocentes",
                column: "DocentesIdUtilizador");

            migrationBuilder.CreateIndex(
                name: "IX_Utilizadores_EscolaFK",
                table: "Utilizadores",
                column: "EscolaFK");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BlocosHorario");

            migrationBuilder.DropTable(
                name: "EscolaUtilizadorComissao");

            migrationBuilder.DropTable(
                name: "UCDocentes");

            migrationBuilder.DropTable(
                name: "Salas");

            migrationBuilder.DropTable(
                name: "Turmas");

            migrationBuilder.DropTable(
                name: "Utilizadores");

            migrationBuilder.DropTable(
                name: "UCs");

            migrationBuilder.DropTable(
                name: "Escolas");
        }
    }
}
