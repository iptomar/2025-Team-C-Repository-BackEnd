using Backend.Data;
using Backend.DTO;
using Backend.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Backend.Controllers.API
{
    [Route("api/[controller]")]
    [ApiController]
    public class ApiSalasController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public ApiSalasController(ApplicationDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Retorna todas as salas.
        /// </summary>
        [HttpGet]
        [Route("GetAll")]
        public async Task<ActionResult<IEnumerable<SalaDTO>>> GetSalas()
        {
            var salas = await _context.Salas
                .Include(s => s.Escola)
                .Select(s => new SalaDTO
                {
                    IdSala = s.IdSala,
                    Nome = s.Nome,
                    Lugares = s.Lugares,
                    TipoSala = s.TipoSala,
                    Localizacao = s.Localizacao,
                    EscolaFK = s.EscolaFK,
                    EscolaNome = s.Escola.Nome
                })
                .ToListAsync();

            return Ok(salas);
        }

        /// <summary>
        /// Retorna todas as salas de uma escola específica.
        /// </summary>
        [HttpGet]
        [Route("GetByEscola/{escolaId}")]
        public async Task<ActionResult<IEnumerable<SalaDTO>>> GetSalasByEscola(int escolaId)
        {
            var salas = await _context.Salas
                .Where(s => s.EscolaFK == escolaId)
                .Include(s => s.Escola)
                .Select(s => new SalaDTO
                {
                    IdSala = s.IdSala,
                    Nome = s.Nome,
                    Lugares = s.Lugares,
                    TipoSala = s.TipoSala,
                    Localizacao = s.Localizacao,
                    EscolaFK = s.EscolaFK,
                    EscolaNome = s.Escola.Nome
                })
                .ToListAsync();

            return Ok(salas);
        }

        /// <summary>
        /// Retorna uma sala pelo ID.
        /// </summary>
        [HttpGet]
        [Route("GetById/{id}")]
        public async Task<ActionResult<SalaDTO>> GetById(int id)
        {
            var sala = await _context.Salas
                .Include(s => s.Escola)
                .FirstOrDefaultAsync(s => s.IdSala == id);

            if (sala == null)
                return NotFound();

            var salaDTO = new SalaDTO
            {
                IdSala = sala.IdSala,
                Nome = sala.Nome,
                Lugares = sala.Lugares,
                TipoSala = sala.TipoSala,
                Localizacao = sala.Localizacao,
                EscolaFK = sala.EscolaFK,
                EscolaNome = sala.Escola?.Nome
            };

            return Ok(salaDTO);
        }

        /// <summary>
        /// Cria uma nova sala.
        /// </summary>
        [HttpPost]
        [Route("Create")]
        public async Task<ActionResult<SalaDTO>> Create([FromBody] SalaDTO salaDTO)
        {
            var escola = await _context.Escolas.FindAsync(salaDTO.EscolaFK);
            if (escola == null)
                return BadRequest(new { message = "Escola não encontrada" });

            var sala = new Sala
            {
                Nome = salaDTO.Nome,
                Lugares = salaDTO.Lugares,
                TipoSala = salaDTO.TipoSala,
                Localizacao = salaDTO.Localizacao,
                EscolaFK = salaDTO.EscolaFK
            };

            _context.Salas.Add(sala);
            await _context.SaveChangesAsync();

            salaDTO.IdSala = sala.IdSala;
            salaDTO.EscolaNome = escola.Nome;

            return CreatedAtAction(nameof(GetById), new { id = sala.IdSala }, salaDTO);
        }

        /// <summary>
        /// Atualiza uma sala existente.
        /// </summary>
        [HttpPut]
        [Route("Update/{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] SalaDTO salaDTO)
        {
            if (id != salaDTO.IdSala)
                return BadRequest();

            var escola = await _context.Escolas.FindAsync(salaDTO.EscolaFK);
            if (escola == null)
                return BadRequest(new { message = "Escola não encontrada" });

            var sala = await _context.Salas.FindAsync(id);
            if (sala == null)
                return NotFound();

            sala.Nome = salaDTO.Nome;
            sala.Lugares = salaDTO.Lugares;
            sala.TipoSala = salaDTO.TipoSala;
            sala.Localizacao = salaDTO.Localizacao;
            sala.EscolaFK = salaDTO.EscolaFK;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.Salas.Any(s => s.IdSala == id))
                    return NotFound();
                else
                    throw;
            }

            return NoContent();
        }

        /// <summary>
        /// Remove uma sala pelo ID.
        /// </summary>
        [HttpDelete]
        [Route("Delete/{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var sala = await _context.Salas.FindAsync(id);
            if (sala == null)
                return NotFound();

            var temBlocosHorario = await _context.BlocosHorario
                .AnyAsync(b => b.SalaFK == id);

            if (temBlocosHorario)
                return BadRequest(new { message = "Não é possível excluir esta sala pois existem blocos de horário associados a ela" });

            _context.Salas.Remove(sala);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
