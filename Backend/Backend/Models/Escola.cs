using System.ComponentModel.DataAnnotations;

namespace Backend.Models
{
    public class Escola
    {
        [Key]
        public int IdEscola { get; set; }
        public string Nome { get; set; } = string.Empty;
        public string Localizacao { get; set; }

        /// <summary>
        /// Lista dos membros da comissão de horários
        /// </summary>
        public ICollection<Utilizador> MembrosComissaoHorarios { get; set; } = new List<Utilizador>();

        /// <summary>
        /// Lista das salas da escola
        /// </summary>
        public ICollection<Sala> Salas { get; set; } = new List<Sala>();
    }
}
