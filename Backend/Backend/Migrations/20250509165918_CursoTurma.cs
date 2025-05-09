using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Backend.Migrations
{
    /// <inheritdoc />
    public partial class CursoTurma : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CursoFK",
                table: "Turmas",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Turmas_CursoFK",
                table: "Turmas",
                column: "CursoFK");

            migrationBuilder.AddForeignKey(
                name: "FK_Turmas_Cursos_CursoFK",
                table: "Turmas",
                column: "CursoFK",
                principalTable: "Cursos",
                principalColumn: "IdCurso",
                onDelete: ReferentialAction.NoAction);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Turmas_Cursos_CursoFK",
                table: "Turmas");

            migrationBuilder.DropIndex(
                name: "IX_Turmas_CursoFK",
                table: "Turmas");

            migrationBuilder.DropColumn(
                name: "CursoFK",
                table: "Turmas");
        }
    }
}
