namespace Backend.DTOs {
    public class UtilizadorDTO {
        public int IdUtilizador { get; set; }
        public string Nome { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Funcao { get; set; } = string.Empty;
        public string? Categoria { get; set; }
        public int? EscolaFK { get; set; }
    }
}