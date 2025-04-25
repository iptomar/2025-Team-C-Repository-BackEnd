using Backend.Data;
using Backend.DTO;
using Backend.Models;
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
        [HttpGet]
        [Route("GetAllEscolas")]
        public async Task<ActionResult<IEnumerable<EscolaDTO>>> GetEscolas()
        {
            var escolas = await _context.Escolas
                .Select(e => new EscolaDTO
                {
                    IdEscola = e.IdEscola,
                    Nome = e.Nome,
                    Localizacao = e.Localizacao
                }).ToListAsync();

            return Ok(escolas);
        }

        /// <summary>
        /// Retorna uma escola pelo ID.
        /// </summary>
        [HttpGet]
        [Route("GetById/{id}")]
        public async Task<ActionResult<EscolaDTO>> GetById(int id)
        {
            var escola = await _context.Escolas.FindAsync(id);

            if (escola == null)
                return NotFound();

            var dto = new EscolaDTO
            {
                IdEscola = escola.IdEscola,
                Nome = escola.Nome,
                Localizacao = escola.Localizacao
            };

            return Ok(dto);
        }

        /// <summary>
        /// Cria uma nova escola.
        /// </summary>
        [HttpPost]
        [Route("Create")]
        public async Task<ActionResult<Escola>> Create([FromBody] Escola escola)
        {
            _context.Escolas.Add(escola);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetById), new { id = escola.IdEscola }, escola);
        }

        /// <summary>
        /// Atualiza uma escola existente.
        /// </summary>
        [HttpPut]
        [Route("Update/{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] Escola escola)
        {
            if (id != escola.IdEscola)
                return BadRequest();

            _context.Entry(escola).State = EntityState.Modified;

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
        [HttpDelete]
        [Route("Delete/{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var escola = await _context.Escolas.FindAsync(id);
            if (escola == null)
                return NotFound();

            _context.Escolas.Remove(escola);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
