using Backend.Models;
using Backend.Data;
using Newtonsoft.Json.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;
using Microsoft.AspNetCore.Identity;

namespace Backend.Controllers.API
{
    [Route("api/[controller]")]
    [ApiController]
    public class ApiExcelUploadController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public ApiExcelUploadController(ApplicationDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        [HttpPost]
        [Route("Upload")]
        public async Task<IActionResult> Upload([FromBody] Dictionary<string, object> sheetsData)
        {
            var docentesImportados = new List<string>();
            var docentesIgnorados = new List<string>();
            List<string> headers = new();

            // ************** FOLHA DE DOCENTES **************
            if (sheetsData != null && sheetsData.TryGetValue("Docentes", out var docentesObj))
            {
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

                if (docentesArray != null && docentesArray.Count > 1)
                {
                    headers = docentesArray[0].Select(x => x?.ToString()).ToList();

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

                        // Verifica se já existe no Identity
                        var identityUser = await _userManager.FindByEmailAsync(email);
                        if (identityUser == null)
                        {
                            identityUser = new IdentityUser
                            {
                                UserName = email,
                                Email = email,
                                EmailConfirmed = true
                            };
                            var result = await _userManager.CreateAsync(identityUser, "123Qwe#");
                            if (!result.Succeeded)
                            {
                                docentesIgnorados.Add($"Linha {i + 1}: Erro ao criar utilizador Identity: {string.Join(", ", result.Errors.Select(e => e.Description))}");
                                continue;
                            }
                            await _userManager.AddToRoleAsync(identityUser, "Docente");
                        }

                        var utilizador = new Utilizador
                        {
                            Nome = nome,
                            Email = email,
                            Funcao = "Docente",
                            UserId = identityUser.Id
                        };

                        await _context.Utilizadores.AddAsync(utilizador);
                        docentesImportados.Add($"{nome} ({email})");
                    }

                    await _context.SaveChangesAsync();
                }
            }

            // ************** FOLHA DE UCS E CURSO **************
            if (sheetsData != null && sheetsData.TryGetValue("UCs", out var ucsObj))
            {
                JArray ucsArray = null;
                if (ucsObj is JsonElement ucsJson && ucsJson.ValueKind == JsonValueKind.Array)
                {
                    ucsArray = JArray.Parse(ucsJson.GetRawText());
                }
                else
                {
                    ucsArray = JToken.FromObject(ucsObj) as JArray;
                }

                if (ucsArray != null && ucsArray.Count > 1)
                {
                    var ucHeaders = ucsArray[0].Select(x => x?.ToString()).ToList();

                    for (int i = 1; i < ucsArray.Count; i++)
                    {
                        var row = ucsArray[i].ToObject<List<object>>();

                        var nomeUC = row[1]?.ToString()?.Trim();
                        var nomeCurso = row[2]?.ToString()?.Trim();
                        int.TryParse(row[3]?.ToString(), out int idCurso);
                        int.TryParse(row[0]?.ToString(), out int idUC);
                        var semestre = row[5]?.ToString()?.Trim();
                        int.TryParse(row[4]?.ToString(), out int ano);

                        if (string.IsNullOrWhiteSpace(nomeUC) || string.IsNullOrWhiteSpace(nomeCurso))
                            continue;

                        // Verifica se o curso existe, senão cria
                        var curso = await _context.Cursos.FirstOrDefaultAsync(c => c.Nome == nomeCurso);
                        if (curso == null)
                        {
                            curso = new Curso
                            {
                                Nome = nomeCurso,
                                EscolaFK = 1
                            };
                            _context.Cursos.Add(curso);
                            await _context.SaveChangesAsync();
                        }

                        // Verifica se UC já existe
                        var ucExistente = await _context.UCs.FirstOrDefaultAsync(uc =>
                            uc.NomeUC == nomeUC && uc.CursoFK == curso.IdCurso);

                        if (ucExistente == null)
                        {
                            ucExistente = new UC
                            {
                                NomeUC = nomeUC,
                                Semestre = semestre,
                                Ano = ano,
                                CursoFK = curso.IdCurso
                            };
                            _context.UCs.Add(ucExistente);
                            await _context.SaveChangesAsync();
                        }
                    }
                }
            }

            // ************** SALAS **************
            if (sheetsData != null && sheetsData.TryGetValue("salas", out var salasObj))
            {
                JArray salasArray = null;
                if (salasObj is JsonElement ucsJson && ucsJson.ValueKind == JsonValueKind.Array)
                {
                    salasArray = JArray.Parse(ucsJson.GetRawText());
                }
                else
                {
                    salasArray = JToken.FromObject(salasObj) as JArray;
                }

                if (salasArray != null && salasArray.Count > 1)
                {
                    var salaHeaders = salasArray[0].Select(x => x?.ToString()).ToList();

                    for (int i = 1; i < salasArray.Count; i++)
                    {
                        var row = salasArray[i].ToObject<List<object>>();

                        var nomeSala = row[2]?.ToString()?.Trim();
                        var lugares = row[7]?.ToString()?.Trim();
                        var tipo = row[1]?.ToString()?.Trim();
                        var localizacao = row[12]?.ToString()?.Trim();
                        int.TryParse(row[11]?.ToString(), out int localizacao_esc);

                        // Workaround para lidar com escolas fora de TMR/ABT
                        if (!(localizacao_esc.Equals(1) | localizacao_esc.Equals(2) | localizacao_esc.Equals(3)))
                        {
                            localizacao_esc = 1;
                        }

                        // Verifica se a sala existe, senão cria
                        var sala = await _context.Salas.FirstOrDefaultAsync(c => c.Nome == nomeSala);
                        if (sala == null)
                        {
                            sala = new Sala
                            {
                                Nome = nomeSala,
                                EscolaFK = localizacao_esc
                            };
                            _context.Salas.Add(sala);
                            await _context.SaveChangesAsync();
                        }
                    }
                }

            }

            return Ok(new
            {
                message = "Folhas processadas com sucesso.",
                docentesImportados,
                docentesIgnorados,
                debug = new
                {
                    headers
                }
            });
        }

    }
}
