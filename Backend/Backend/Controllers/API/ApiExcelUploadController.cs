using Backend.Models;
using Backend.Data;
using Newtonsoft.Json.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace Backend.Controllers.API
{
    [Route("api/[controller]")]
    [ApiController]
    public class ApiExcelUploadController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public ApiExcelUploadController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpPost]
        [Route("Upload")]
        public async Task<IActionResult> Upload([FromBody] Dictionary<string, object> sheetsData)
        {
            if (sheetsData == null || !sheetsData.TryGetValue("Docentes", out var docentesObj))
                return BadRequest(new { message = "Nenhuma folha 'Docentes' recebida." });

            // Suporte robusto para JsonElement e outros tipos
            JArray docentesArray = null;
            if (docentesObj is JsonElement jsonElement && jsonElement.ValueKind == JsonValueKind.Array)
            {
                docentesArray = JArray.Parse(jsonElement.GetRawText());
            }
            else
            {
                docentesArray = JToken.FromObject(docentesObj) as JArray;
            }

            if (docentesArray == null || docentesArray.Count <= 1)
                return BadRequest(new { message = "Nenhum docente para importar." });

            var headers = docentesArray[0].Select(x => x?.ToString()).ToList();
            var docentesImportados = new List<string>();
            var docentesIgnorados = new List<string>();

            for (int i = 1; i < docentesArray.Count; i++)
            {
                var row = docentesArray[i].ToObject<List<object>>();

                if (row.Count < 5)
                {
                    docentesIgnorados.Add($"Linha {i + 1}: Dados insuficientes.");
                    continue;
                }

                var nome = row[1]?.ToString()?.Trim();
                var email = row[2]?.ToString()?.Trim();

                if (string.IsNullOrWhiteSpace(nome) || string.IsNullOrWhiteSpace(email))
                {
                    docentesIgnorados.Add($"Linha {i + 1}: Nome ou email em falta.");
                    continue;
                }

                var existe = await _context.Utilizadores.AnyAsync(u => u.Email == email);
                if (existe)
                {
                    docentesIgnorados.Add($"Linha {i + 1}: Email '{email}' já existe.");
                    continue;
                }

                var utilizador = new Utilizador
                {
                    Nome = nome,
                    Email = email,
                    Funcao = "Docente",
                    UserId = ""
                };

                await _context.Utilizadores.AddAsync(utilizador);
                docentesImportados.Add($"{nome} ({email})");
            }

            await _context.SaveChangesAsync();

            return Ok(new
            {
                message = "Folhas processadas com sucesso.",
                docentesImportados,
                docentesIgnorados,
                debug = new
                {
                    headers,
                    sampleRow = docentesArray.Count > 1 ? docentesArray[1] : null
                }
            });
        }
    }
}
