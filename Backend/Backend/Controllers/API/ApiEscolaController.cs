using Backend.Data;
using Backend.DTO;
using Backend.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Backend.Controllers.API
{
    [Route("api/[controller]")]
    [ApiController]
    public class ApiEscolaController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public ApiEscolaController(ApplicationDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Retorna todas as escolas.
        /// </summary>
        [Authorize]
        [HttpGet]
        [Route("GetAllEscolas")]
        public async Task<ActionResult<IEnumerable<EscolaDTO>>> GetEscolas()
        {
            var escolas = await _context.Escolas
                .Select(e => new EscolaDTO
                {
                    IdEscola = e.IdEscola,
                    Nome = e.Nome,
                    Localizacao = e.Localizacao,
                    TotalSalas = e.Salas.Count
                }).ToListAsync();

            return Ok(escolas);
        }

        /// <summary>
        /// Retorna uma escola pelo ID.
        /// </summary>
        [Authorize]
        [HttpGet]
        [Route("GetById/{id}")]
        public async Task<ActionResult<EscolaDTO>> GetById(int id)
        {
            var escola = await _context.Escolas
                .Include(e => e.Salas)
                .FirstOrDefaultAsync(e => e.IdEscola == id);

            if (escola == null)
                return NotFound();

            var dto = new EscolaDTO
            {
                IdEscola = escola.IdEscola,
                Nome = escola.Nome,
                Localizacao = escola.Localizacao,
                TotalSalas = escola.Salas.Count
            };

            return Ok(dto);
        }

        /// <summary>
        /// Cria uma nova escola.
        /// </summary>
        [Authorize(Roles = "Administrador")]
        [HttpPost]
        [Route("Create")]
        public async Task<ActionResult<EscolaDTO>> Create([FromBody] EscolaDTO escolaDTO)
        {
            var escola = new Escola
            {
                Nome = escolaDTO.Nome,
                Localizacao = escolaDTO.Localizacao
            };

            _context.Escolas.Add(escola);
            await _context.SaveChangesAsync();

            escolaDTO.IdEscola = escola.IdEscola;

            return CreatedAtAction(nameof(GetById), new { id = escola.IdEscola }, escolaDTO);
        }

        /// <summary>
        /// Atualiza uma escola existente.
        /// </summary>
        [Authorize(Roles = "Administrador,MembroComissao")]
        [HttpPut]
        [Route("Update/{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] EscolaDTO escolaDTO)
        {
            if (id != escolaDTO.IdEscola)
                return BadRequest();

            var escola = await _context.Escolas.FindAsync(id);
            if (escola == null)
                return NotFound();

            escola.Nome = escolaDTO.Nome;
            escola.Localizacao = escolaDTO.Localizacao;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.Escolas.Any(e => e.IdEscola == id))
                    return NotFound();
                else
                    throw;
            }

            return NoContent();
        }

        /// <summary>
        /// Remove uma escola pelo ID.
        /// </summary>
        [Authorize(Roles = "Administrador")]
        [HttpDelete]
        [Route("Delete/{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var escola = await _context.Escolas
                .Include(e => e.Salas)
                .FirstOrDefaultAsync(e => e.IdEscola == id);

            if (escola == null)
                return NotFound();

            // Verificar se existem salas associadas à escola
            if (escola.Salas.Any())
                return BadRequest(new { message = "Não é possível excluir esta escola pois existem salas associadas a ela" });

            _context.Escolas.Remove(escola);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
