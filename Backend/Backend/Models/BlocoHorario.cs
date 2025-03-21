using System.ComponentModel.DataAnnotations.Schema;

namespace Backend.Models
{
    public class BlocoHorario
    {

        public int IdBloco { get; set; }

        public TimeSpan HoraInicio { get; set; }

        public TimeSpan HoraFim { get; set; }

        public DayOfWeek DiaSemana { get; set; }

        // FK para referenciar o professor do bloco
        [ForeignKey(nameof(Professor))]
        public int ProfessorFK { get; set; }
        public Utilizador Professor { get; set; }

        // FK para referenciar a disciplina do bloco
        [ForeignKey(nameof(Disciplina))]
        public int DisciplinaFK { get; set; }
        public UC Disciplina { get; set; }

        // FK para referenciar a sala do bloco
        [ForeignKey(nameof(Sala))]
        public int SalaFK { get; set; }
        public Sala Sala { get; set; }

        // FK para referenciar a turma do bloco
        [ForeignKey(nameof(Turma))]
        public int TurmaFK { get; set; }
        public Turma Turma { get; set; }

        // FK para referenciar a tipologia do bloco
        [ForeignKey("Tipologia")]
        public int TipologiaFK { get; set; }
        public UC Tipologia { get; set; }

    }
}
