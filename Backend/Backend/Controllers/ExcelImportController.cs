using Backend.Data;
using Backend.Models;
using Microsoft.AspNetCore.Mvc;
using OfficeOpenXml;

namespace Backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ExcelImportController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public ExcelImportController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpPost("upload")]
        public async Task<IActionResult> UploadExcel(IFormFile file)
        {
            if (file == null || file.Length <= 0)
                return BadRequest("Ficheiro inválido.");

            using var stream = new MemoryStream();
            await file.CopyToAsync(stream);
            using var package = new ExcelPackage(stream);

            try
            {
                // Importar escolas
                var escolaSheet = package.Workbook.Worksheets.FirstOrDefault(ws => ws.Name == "Escolas");
                if (escolaSheet != null)
                {
                    for (int row = 2; row <= escolaSheet.Dimension.Rows; row++)
                    {
                        var escola = new Escola
                        {
                            Nome = escolaSheet.Cells[row, 1].Text,
                            Localizacao = escolaSheet.Cells[row, 2].Text
                        };
                        _context.Escolas.Add(escola);
                    }
                }

                // Importar salas
                var salaSheet = package.Workbook.Worksheets.FirstOrDefault(ws => ws.Name == "Salas");
                if (salaSheet != null)
                {
                    for (int row = 2; row <= salaSheet.Dimension.Rows; row++)
                    {
                        var sala = new Sala
                        {
                            Nome = salaSheet.Cells[row, 1].Text,
                            Lugares = int.TryParse(salaSheet.Cells[row, 2].Text, out var lugares) ? lugares : 0,
                            TipoSala = salaSheet.Cells[row, 3].Text,
                            Localizacao = salaSheet.Cells[row, 4].Text,
                            EscolaFK = int.Parse(salaSheet.Cells[row, 5].Text)
                        };
                        _context.Salas.Add(sala);
                    }
                }

                // Importar disciplinas
                var ucSheet = package.Workbook.Worksheets.FirstOrDefault(ws => ws.Name == "UCs");
                if (ucSheet != null)
                {
                    for (int row = 2; row <= ucSheet.Dimension.Rows; row++)
                    {
                        var uc = new UC
                        {
                            NomeDisciplina = ucSheet.Cells[row, 13].Text,
                            TipoDisciplina = ucSheet.Cells[row, 12].Text,
                            GrauAcademico = ucSheet.Cells[row, 4].Text,
                            Tipologia = ucSheet.Cells[row, 4].Text,
                            Semestre = ucSheet.Cells[row, 11].Text
                        };
                        _context.UCs.Add(uc);
                    }
                }

                // Importar turmas
                var turmaSheet = package.Workbook.Worksheets.FirstOrDefault(ws => ws.Name == "Turmas");
                if (turmaSheet != null)
                {
                    for (int row = 2; row <= turmaSheet.Dimension.Rows; row++)
                    {
                        var turma = new Turma
                        {
                            Nome = turmaSheet.Cells[row, 1].Text,
                            DisciplinaFK = int.Parse(turmaSheet.Cells[row, 2].Text)
                        };
                        _context.Turmas.Add(turma);
                    }
                }

                // Importar utilizadores
                var userSheet = package.Workbook.Worksheets.FirstOrDefault(ws => ws.Name == "Docentes");
                if (userSheet != null)
                {
                    for (int row = 2; row <= userSheet.Dimension.Rows; row++)
                    {
                        var utilizador = new Utilizador
                        {
                            Nome = userSheet.Cells[row, 2].Text,
                            Email = userSheet.Cells[row, 3].Text,
                            
                            CategoriaId = int.TryParse(userSheet.Cells[row, 6].Text, out var catId) ? catId : null,
                            Categoria = userSheet.Cells[row, 7].Text,
                            EscolaFK = int.TryParse(userSheet.Cells[row, 6].Text, out var escId) ? escId : null,
                            UserId = userSheet.Cells[row, 1].Text
                        };
                        _context.Utilizadores.Add(utilizador);
                    }
                }

                await _context.SaveChangesAsync();
                return Ok("Dados importados com sucesso.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erro ao importar: {ex.Message}");
            }
        }
    }
}