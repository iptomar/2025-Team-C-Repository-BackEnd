using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel;

namespace Backend.Models
{
    public class Curso
    {
        [Key]
        public int IdCurso { get; set; }

        [DisplayName("Nome do Curso")]
        public string Nome { get; set; }

        [DisplayName("Grau")]
        [RegularExpression(@"^(Licenciatura|Mestrado|Doutoramento|Pós-Graduação|Microcredenciação|Técnico Superior Profissional)$",
            ErrorMessage = "O Grau deve ser um dos seguintes: Licenciatura, Mestrado, Doutoramento, Pós-Graduação, Microcredenciação ou Técnico Superior Profissional")]
        public string Grau { get; set; }

        // FK para referenciar a escola do curso
        [ForeignKey(nameof(Escola))]
        [DisplayName("Escola")]
        public int EscolaFK { get; set; }
        public Escola Escola { get; set; }

        // Lista de UCs que compõem o curso
        public ICollection<UC> UnidadesCurriculares { get; set; }
    }
}
