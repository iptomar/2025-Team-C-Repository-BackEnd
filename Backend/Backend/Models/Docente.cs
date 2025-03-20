using System.ComponentModel.DataAnnotations;


namespace Backend.Models
{
    public class Docente
    {
        [Key]
        public int IdDocente { get; set; }
        public string Nome { get; set; }
        public string Email { get; set; }
       // public int UnidadeDepartamentalId { get; set; }
        public int CategoriaId { get; set; }
        public string Categoria { get; set; }
    }
}
