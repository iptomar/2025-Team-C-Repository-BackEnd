using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Backend.Models
{
    public class Utilizador
    {
        [Key]
        public int IdUtilizador { get; set; }
        public string Nome { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;

        // Funcão/Perfil: Administrador, MembroComissao, Docente
        public string Funcao { get; set; } = string.Empty;

        // Campos específicos para docentes
        public int? CategoriaId { get; set; }
        public string Categoria { get; set; }

        // FK para a escola principal
        [ForeignKey(nameof(Escola))]
        public int? EscolaFK { get; set; }
        public Escola Escola { get; set; }

        /// <summary>
        /// Lista das escolas onde o utilizador ensina (caso seja docente)
        /// </summary>
        public ICollection<Escola> EscolasOndeEnsina { get; set; }

        /// <summary>
        /// Lista das disciplinas lecionadas (caso seja docente)
        /// </summary>
        public ICollection<UC> DisciplinasLecionadas { get; set; }

        /// <summary>
        /// Lista dos blocos de horário associados (caso seja docente)
        /// </summary>
        public ICollection<BlocoHorario> BlocosHorario { get; set; }
    }
}
