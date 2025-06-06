﻿using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Backend.Models
{
    public class Sala
    {
        [Key]
        public int IdSala { get; set; }

        public string Nome { get; set; } = string.Empty;

        public int Lugares { get; set; }

        public string TipoSala { get; set; } = string.Empty; // laboratório, sala de aula, auditório, etc.

        public string Localizacao { get; set; } = string.Empty; // Abrantes, Tomar, Mafra... (nome_2) 

        // FK para referenciar a escola à qual a sala pertence
        [ForeignKey(nameof(Escola))]
        public int EscolaFK { get; set; }
        public Escola Escola { get; set; }

        /// <summary>
        /// Lista dos blocos de horário associados a esta sala
        /// </summary>
        public ICollection<BlocoHorario> BlocosHorario { get; set; } = new List<BlocoHorario>();
    }
}
