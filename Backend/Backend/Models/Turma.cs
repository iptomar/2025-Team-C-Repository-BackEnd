﻿using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Backend.Models
{
    public class Turma
    {
        [Key]
        public int IdTurma { get; set; }
        public string Nome { get; set; } = string.Empty; // Turma 1, Turma A, etc.

        // FK para referenciar a curso da turma
        [ForeignKey(nameof(Curso))]
        public int CursoFK { get; set; }
        public Curso Curso { get; set; }

        /// <summary>
        /// Lista dos blocos de horário associados a esta turma
        /// </summary>
        public ICollection<BlocoHorario> BlocosHorario { get; set; } = new List<BlocoHorario>();
    }
}
