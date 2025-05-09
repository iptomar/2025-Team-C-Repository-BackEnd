using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Backend.Models
{
    public class UC
    {
        [Key]
        public int IdUC { get; set; }
        public string NomeUC { get; set; } = string.Empty;
        public string TipoUC { get; set; } = string.Empty; //teórica / teórico pratica / prática / laboratorial etc.
        public string GrauAcademico { get; set; } = string.Empty; //Licenciatura, Mestrado, Doutoramento, etc
        public string? Tipologia { get; set; } = string.Empty;
        public string Semestre { get; set; } = string.Empty;

        [RegularExpression(@"^[1-4]$", ErrorMessage = "O ano deve ser um valor de 1 a 4.")]
        public int? Ano { get; set; } // 1º ano, 2º ano, etc.

        /// <summary>
        /// Lista dos docentes que lecionam esta disciplina
        /// </summary>
        public ICollection<Utilizador> Docentes { get; set; } = new List<Utilizador>();

        /// <summary>
        /// Lista das turmas desta disciplina
        /// </summary>
        public ICollection<Turma> Turmas { get; set; } = new List<Turma>();

        // FK para referenciar o curso da disciplina
        [ForeignKey(nameof(Curso))]
        [DisplayName("Curso")]
        public int CursoFK { get; set; }
        public Curso Curso { get; set; }
    }
}
