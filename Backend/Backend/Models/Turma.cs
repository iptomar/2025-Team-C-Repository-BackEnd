using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Backend.Models
{
    public class Turma
    {
        [Key]
        public int IdTurma { get; set; }
        public string Nome { get; set; } = string.Empty; // Turma 1, Turma A, etc.

        // FK para referenciar a disciplina da turma
        [ForeignKey(nameof(Disciplina))]
        public int DisciplinaFK { get; set; }
        public UC Disciplina { get; set; }

        /// <summary>
        /// Lista dos blocos de horário associados a esta turma
        /// </summary>
        public ICollection<BlocoHorario> BlocosHorario { get; set; } = new List<BlocoHorario>();
    }
}
