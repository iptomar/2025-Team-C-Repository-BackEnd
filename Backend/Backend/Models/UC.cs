using System.ComponentModel.DataAnnotations;


namespace Backend.Models
{
    public class UC
    {
        [Key]
        public int IdDisciplina { get; set; }
        public string NomeDisciplina { get; set; }
        public string TipoDisciplina { get; set; } //Obrigatória, Opcional, etc
        public string DS_Grau { get; set; } //Licenciatura, Mestrado, Doutoramento, etc
        public string Tipologia { get; set; }
        public string Semestre { get; set; }

    }
}
