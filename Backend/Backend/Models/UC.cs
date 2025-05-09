using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Backend.Models
{
    public class UC
    {
        [Key]
        public int IdDisciplina { get; set; }
        public string NomeDisciplina { get; set; } = string.Empty;
        public string TipoDisciplina { get; set; } = string.Empty; //teórica / teórico pratica / prática / laboratorial etc.
        public string GrauAcademico { get; set; } = string.Empty; //Licenciatura, Mestrado, Doutoramento, etc
        //TODO: ISTO É NECESSÁRIO?
        // public string Tipologia { get; set; } = string.Empty;
        public string Semestre { get; set; } = string.Empty;


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
