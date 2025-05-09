namespace Backend.DTO
{
    public class TurmaDTO
    {
        public int IdTurma { get; set; }
        public string Nome { get; set; }
        public int CursoFK { get; set; }
        public string CursoNome { get; set; }
    }
}
