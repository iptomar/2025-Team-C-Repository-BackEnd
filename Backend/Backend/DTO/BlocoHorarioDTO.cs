namespace Backend.DTO
{
    public class BlocoHorarioDTO
    {
        public int IdBloco { get; set; }
        public TimeSpan HoraInicio { get; set; }
        public TimeSpan HoraFim { get; set; }
        public DateOnly Dia { get; set; }

        // Relacionamentos
        public int ProfessorFK { get; set; }
        public string ProfessorNome { get; set; } // Nome do professor

        public int UnidadeCurricularFK { get; set; }
        public string UnidadeCurricularNome { get; set; } // Nome da UC

        public int SalaFK { get; set; }
        public string SalaNome { get; set; } // Nome da sala

        public int TurmaFK { get; set; }
        public string TurmaNome { get; set; } // Nome da turma
    }
}
