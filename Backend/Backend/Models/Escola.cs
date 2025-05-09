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
        public ICollection<Utilizador> MembrosComissaoHorarios { get; set; } = new List<Utilizador>(); //TODO: isto poderia ser a lista de todos os utilizadores de uma escola!?
                                                                                                       // em vez de existir a lista das escolasOndeEnsina no utilizador?

        /// <summary>
        /// Lista das salas da escola
        /// </summary>
        public ICollection<Sala> Salas { get; set; } = new List<Sala>();
    }
}
