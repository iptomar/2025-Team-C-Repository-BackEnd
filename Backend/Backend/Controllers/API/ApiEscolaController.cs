using Backend.Data;
using Backend.DTO;
using Backend.Models;
using Microsoft.AspNetCore.Mvc;

namespace Backend.Controllers.API
{
    [Route("api/[controller]")]
    [ApiController]
    public class ApiEscolaController : ControllerBase {
        // ApplicationDbContext é a classe que representa a base de dados
        private readonly ApplicationDbContext _context;

        public ApiEscolaController(ApplicationDbContext context) {
            _context = context;
        }

        [HttpGet]
        [Route("GetAllEscolas")]
        public ActionResult<IEnumerable<EscolaDTO>> GetEscolas() {

            // Obter todas as escolas da base de dados
            var escolas = _context.Escolas.Select(
                e => new EscolaDTO {
                    IdEscola = e.IdEscola,
                    Nome = e.Nome,
                    Localizacao = e.Localizacao
                }).ToList();
            return Ok(escolas);
        }
    }
}
