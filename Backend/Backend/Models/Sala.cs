using System.ComponentModel.DataAnnotations;


namespace Backend.Models
{
    public class Sala
    {
        [Key]
        public int IdSala { get; set; }

        public string Nome { get; set; }

        public int Lugares { get; set; }

        public string TipoSala { get; set; }

        public string  Localizacao { get; set; }

    }
}
