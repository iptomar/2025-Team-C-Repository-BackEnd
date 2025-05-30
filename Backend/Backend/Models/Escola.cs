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
        /// Lista dos membros da escola
        /// </summary>
        public ICollection<Utilizador> Membros { get; set; } = new List<Utilizador>(); 

        /// <summary>
        /// Lista das salas da escola
        /// </summary>
        public ICollection<Sala> Salas { get; set; } = new List<Sala>();
    }
}
