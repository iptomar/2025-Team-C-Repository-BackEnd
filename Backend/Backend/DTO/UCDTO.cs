namespace Backend.DTO
{
    public class UCDTO
    {
        public int IdUC { get; set; }
        public string NomeUC { get; set; }
        public string TipoUC { get; set; }
        public string GrauAcademico { get; set; }
        public string? Tipologia { get; set; }
        public string Semestre { get; set; }
        public int? Ano { get; set; }
        public int CursoFK { get; set; }
    }
}
