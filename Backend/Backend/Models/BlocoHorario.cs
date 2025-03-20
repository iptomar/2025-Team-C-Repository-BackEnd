namespace Backend.Models
{
    public class BlocoHorario
    {

        public int IdBloco { get; set; }

        public TimeSpan HoraInicio { get; set; }

        public TimeSpan HoraFim { get; set; }

        public Docente Professor { get; set; }

        public UC Disciplina { get; set; }

        public Sala Sala { get; set; }

        public UC Tipologia { get; set; }

        public string DiaSemana { get; set; }

    }
}
