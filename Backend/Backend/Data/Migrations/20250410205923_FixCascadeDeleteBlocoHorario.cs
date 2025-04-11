using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Backend_v02.Data.Migrations
{
    /// <inheritdoc />
    public partial class FixCascadeDeleteBlocoHorario : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BlocosHorario_Salas_SalaFK",
                table: "BlocosHorario");

            migrationBuilder.DropForeignKey(
                name: "FK_BlocosHorario_Turmas_TurmaFK",
                table: "BlocosHorario");

            migrationBuilder.DropForeignKey(
                name: "FK_BlocosHorario_UCs_DisciplinaFK",
                table: "BlocosHorario");

            migrationBuilder.DropForeignKey(
                name: "FK_BlocosHorario_Utilizadores_ProfessorFK",
                table: "BlocosHorario");

            migrationBuilder.AddColumn<int>(
                name: "SalaIdSala",
                table: "BlocosHorario",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "TurmaIdTurma",
                table: "BlocosHorario",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "UtilizadorIdUtilizador",
                table: "BlocosHorario",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_BlocosHorario_SalaIdSala",
                table: "BlocosHorario",
                column: "SalaIdSala");

            migrationBuilder.CreateIndex(
                name: "IX_BlocosHorario_TurmaIdTurma",
                table: "BlocosHorario",
                column: "TurmaIdTurma");

            migrationBuilder.CreateIndex(
                name: "IX_BlocosHorario_UtilizadorIdUtilizador",
                table: "BlocosHorario",
                column: "UtilizadorIdUtilizador");

            migrationBuilder.AddForeignKey(
                name: "FK_BlocosHorario_Salas_SalaFK",
                table: "BlocosHorario",
                column: "SalaFK",
                principalTable: "Salas",
                principalColumn: "IdSala",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_BlocosHorario_Salas_SalaIdSala",
                table: "BlocosHorario",
                column: "SalaIdSala",
                principalTable: "Salas",
                principalColumn: "IdSala");

            migrationBuilder.AddForeignKey(
                name: "FK_BlocosHorario_Turmas_TurmaFK",
                table: "BlocosHorario",
                column: "TurmaFK",
                principalTable: "Turmas",
                principalColumn: "IdTurma",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_BlocosHorario_Turmas_TurmaIdTurma",
                table: "BlocosHorario",
                column: "TurmaIdTurma",
                principalTable: "Turmas",
                principalColumn: "IdTurma");

            migrationBuilder.AddForeignKey(
                name: "FK_BlocosHorario_UCs_DisciplinaFK",
                table: "BlocosHorario",
                column: "DisciplinaFK",
                principalTable: "UCs",
                principalColumn: "IdDisciplina",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_BlocosHorario_Utilizadores_ProfessorFK",
                table: "BlocosHorario",
                column: "ProfessorFK",
                principalTable: "Utilizadores",
                principalColumn: "IdUtilizador",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_BlocosHorario_Utilizadores_UtilizadorIdUtilizador",
                table: "BlocosHorario",
                column: "UtilizadorIdUtilizador",
                principalTable: "Utilizadores",
                principalColumn: "IdUtilizador");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BlocosHorario_Salas_SalaFK",
                table: "BlocosHorario");

            migrationBuilder.DropForeignKey(
                name: "FK_BlocosHorario_Salas_SalaIdSala",
                table: "BlocosHorario");

            migrationBuilder.DropForeignKey(
                name: "FK_BlocosHorario_Turmas_TurmaFK",
                table: "BlocosHorario");

            migrationBuilder.DropForeignKey(
                name: "FK_BlocosHorario_Turmas_TurmaIdTurma",
                table: "BlocosHorario");

            migrationBuilder.DropForeignKey(
                name: "FK_BlocosHorario_UCs_DisciplinaFK",
                table: "BlocosHorario");

            migrationBuilder.DropForeignKey(
                name: "FK_BlocosHorario_Utilizadores_ProfessorFK",
                table: "BlocosHorario");

            migrationBuilder.DropForeignKey(
                name: "FK_BlocosHorario_Utilizadores_UtilizadorIdUtilizador",
                table: "BlocosHorario");

            migrationBuilder.DropIndex(
                name: "IX_BlocosHorario_SalaIdSala",
                table: "BlocosHorario");

            migrationBuilder.DropIndex(
                name: "IX_BlocosHorario_TurmaIdTurma",
                table: "BlocosHorario");

            migrationBuilder.DropIndex(
                name: "IX_BlocosHorario_UtilizadorIdUtilizador",
                table: "BlocosHorario");

            migrationBuilder.DropColumn(
                name: "SalaIdSala",
                table: "BlocosHorario");

            migrationBuilder.DropColumn(
                name: "TurmaIdTurma",
                table: "BlocosHorario");

            migrationBuilder.DropColumn(
                name: "UtilizadorIdUtilizador",
                table: "BlocosHorario");

            migrationBuilder.AddForeignKey(
                name: "FK_BlocosHorario_Salas_SalaFK",
                table: "BlocosHorario",
                column: "SalaFK",
                principalTable: "Salas",
                principalColumn: "IdSala",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_BlocosHorario_Turmas_TurmaFK",
                table: "BlocosHorario",
                column: "TurmaFK",
                principalTable: "Turmas",
                principalColumn: "IdTurma",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_BlocosHorario_UCs_DisciplinaFK",
                table: "BlocosHorario",
                column: "DisciplinaFK",
                principalTable: "UCs",
                principalColumn: "IdDisciplina",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_BlocosHorario_Utilizadores_ProfessorFK",
                table: "BlocosHorario",
                column: "ProfessorFK",
                principalTable: "Utilizadores",
                principalColumn: "IdUtilizador",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
