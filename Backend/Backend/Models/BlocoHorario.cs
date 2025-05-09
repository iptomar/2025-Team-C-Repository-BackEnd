using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace Backend.Models
{
    public class BlocoHorario
    {
        [Key]
        public int IdBloco { get; set; }

        [DisplayName("Hora de Início")]
        public TimeSpan HoraInicio { get; set; }

        [DisplayName("Hora de Fim")]
        public TimeSpan HoraFim { get; set; }

        [DisplayName("Dia")]
        public DateOnly Dia { get; set; }

        // FK para referenciar o professor do bloco
        [ForeignKey(nameof(Professor))]
        [DisplayName("Professor")]
        public int ProfessorFK { get; set; }
        public Utilizador Professor { get; set; }

        // FK para referenciar a UC do bloco
        [ForeignKey(nameof(UnidadeCurricular))]
        [DisplayName("UnidadeCurricular")]
        public int UnidadeCurricularFK { get; set; }
        public UC UnidadeCurricular { get; set; }

        // FK para referenciar a sala do bloco
        [ForeignKey(nameof(Sala))]
        [DisplayName("Sala")]
        public int SalaFK { get; set; }
        public Sala Sala { get; set; }

        // FK para referenciar a turma do bloco
        [ForeignKey(nameof(Turma))]
        [DisplayName("Turma")]
        public int TurmaFK { get; set; }
        public Turma Turma { get; set; }
    }
}
