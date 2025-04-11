using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace Backend.Models
{
    public class Sala
    {
        [Key]
        public int IdSala { get; set; }

        [DisplayName("Nome da Sala")]
        public string Nome { get; set; } = string.Empty;

        public int Lugares { get; set; }

        [DisplayName("Tipo de Sala")]
        public string TipoSala { get; set; } = string.Empty;

        public string Localizacao { get; set; } = string.Empty;

        // FK para referenciar a escola à qual a sala pertence
        [ForeignKey(nameof(Escola))]
        [DisplayName("Escola")]
        public int EscolaFK { get; set; }
        public Escola Escola { get; set; }

        /// <summary>
        /// Lista dos blocos de horário associados a esta sala
        /// </summary>
        public ICollection<BlocoHorario> BlocosHorario { get; set; } = new List<BlocoHorario>();
    }
}
