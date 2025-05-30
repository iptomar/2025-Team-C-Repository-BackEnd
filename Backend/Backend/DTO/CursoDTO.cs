namespace Backend.DTO
{
    public class CursoDTO
    {
        public int IdCurso { get; set; }
        public string Nome { get; set; }
        public string Grau { get; set; }
        public int EscolaFK { get; set; }
        public string EscolaNome { get; set; }
    }
}