﻿using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Backend.Models
{
    public class UC
    {
        [Key]
        public int IdDisciplina { get; set; }

        [DisplayName("Nome da Disciplina")]
        public string NomeDisciplina { get; set; } = string.Empty;

        [DisplayName("Tipo de Disciplina")]
        public string TipoDisciplina { get; set; } = string.Empty; //TODO: isto aqui é o quê? (estava "Obrigatória, Opcional") - será preciso?

        [DisplayName("Grau Académico")]
        public string GrauAcademico { get; set; } = string.Empty; //Licenciatura, Mestrado, Doutoramento, etc
        public string Tipologia { get; set; } = string.Empty;
        public string Semestre { get; set; } = string.Empty;


        /// <summary>
        /// Lista dos docentes que lecionam esta disciplina
        /// </summary>
        public ICollection<Utilizador> Docentes { get; set; } = new List<Utilizador>();

        /// <summary>
        /// Lista das turmas desta disciplina
        /// </summary>
        public ICollection<Turma> Turmas { get; set; } = new List<Turma>();
    }
}
