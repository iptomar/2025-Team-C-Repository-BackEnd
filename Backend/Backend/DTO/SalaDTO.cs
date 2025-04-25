namespace Backend.DTO
{
    public class SalaDTO
    {
        public int IdSala { get; set; }
        public string Nome { get; set; }
        public int Lugares { get; set; }
        public string TipoSala { get; set; }
        public string Localizacao { get; set; }
        public int EscolaFK { get; set; }
        public string EscolaNome { get; set; }
    }
}