using System.Collections.Generic;

namespace Backend.DTO
{
    public class TurmaDTO
    {
        public int IdTurma { get; set; }
        public string Nome { get; set; }
        public int DisciplinaFK { get; set; }
        public string DisciplinaNome { get; set; }
    }
}