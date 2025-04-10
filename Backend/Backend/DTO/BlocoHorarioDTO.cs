using System;

namespace Backend.DTO
{
    public class BlocoHorarioDTO
    {
        public int IdBloco { get; set; }
        public TimeSpan HoraInicio { get; set; }
        public TimeSpan HoraFim { get; set; }
        public DayOfWeek DiaSemana { get; set; }
        public string ProfessorNome { get; set; }
        public string DisciplinaNome { get; set; }
        public string SalaNome { get; set; }
        public string Tipologia { get; set; }
        public string TurmaNome { get; set; }
    }
}
