using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace Backend.Models
{
    public class Turma
    {
        [Key]
        public int IdTurma { get; set; }
        public string Nome { get; set; } = string.Empty;

        // FK para referenciar a disciplina da turma
        [ForeignKey(nameof(Disciplina))]
        [DisplayName("Disciplina")]
        public int DisciplinaFK { get; set; }
        public UC Disciplina { get; set; }

        /// <summary>
        /// Lista dos blocos de horário associados a esta turma
        /// </summary>
        public ICollection<BlocoHorario> BlocosHorario { get; set; } = new List<BlocoHorario>();
    }
}
