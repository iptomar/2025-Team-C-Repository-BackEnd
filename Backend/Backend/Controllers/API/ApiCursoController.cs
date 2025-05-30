using Backend.Data;
using Backend.DTO;
using Backend.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Backend.Controllers.API
{
    [Route("api/[controller]")]
    [ApiController]
    public class ApiCursoController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public ApiCursoController(ApplicationDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Retorna todos os cursos.
        /// </summary>
        [HttpGet]
        [Route("GetAll")]
        public async Task<ActionResult<IEnumerable<CursoDTO>>> GetCursos()
        {
            var cursos = await _context.Cursos
                .Include(c => c.Escola)
                .Select(c => new CursoDTO
                {
                    IdCurso = c.IdCurso,
                    Nome = c.Nome,
                    Grau = c.Grau,
                    EscolaFK = c.EscolaFK,
                    EscolaNome = c.Escola.Nome
                })
                .ToListAsync();

            return Ok(cursos);
        }

        /// <summary>
        /// Retorna um curso pelo ID.
        /// </summary>
        [HttpGet]
        [Route("GetById/{id}")]
        public async Task<ActionResult<CursoDTO>> GetById(int id)
        {
            var curso = await _context.Cursos
                .Include(c => c.Escola)
                .FirstOrDefaultAsync(c => c.IdCurso == id);

            if (curso == null)
                return NotFound();

            var dto = new CursoDTO
            {
                IdCurso = curso.IdCurso,
                Nome = curso.Nome,
                Grau = curso.Grau,
                EscolaFK = curso.EscolaFK,
                EscolaNome = curso.Escola?.Nome
            };

            return Ok(dto);
        }

        /// <summary>
        /// Cria um novo curso.
        /// </summary>
        [HttpPost]
        [Route("Create")]
        public async Task<ActionResult<CursoDTO>> Create([FromBody] CursoDTO cursoDTO)
        {
            var escola = await _context.Escolas.FindAsync(cursoDTO.EscolaFK);
            if (escola == null)
                return BadRequest(new { message = "Escola não encontrada" });

            var curso = new Curso
            {
                Nome = cursoDTO.Nome,
                Grau = cursoDTO.Grau,
                EscolaFK = cursoDTO.EscolaFK
            };

            _context.Cursos.Add(curso);
            await _context.SaveChangesAsync();

            cursoDTO.IdCurso = curso.IdCurso;
            cursoDTO.EscolaNome = escola.Nome;

            return CreatedAtAction(nameof(GetById), new { id = curso.IdCurso }, cursoDTO);
        }

        /// <summary>
        /// Atualiza um curso existente.
        /// </summary>
        [HttpPut]
        [Route("Update/{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] CursoDTO cursoDTO)
        {
            if (id != cursoDTO.IdCurso)
                return BadRequest();

            var escola = await _context.Escolas.FindAsync(cursoDTO.EscolaFK);
            if (escola == null)
                return BadRequest(new { message = "Escola não encontrada" });

            var curso = await _context.Cursos.FindAsync(id);
            if (curso == null)
                return NotFound();

            curso.Nome = cursoDTO.Nome;
            curso.Grau = cursoDTO.Grau;
            curso.EscolaFK = cursoDTO.EscolaFK;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.Cursos.Any(c => c.IdCurso == id))
                    return NotFound();
                else
                    throw;
            }

            return NoContent();
        }

        /// <summary>
        /// Remove um curso pelo ID.
        /// </summary>
        [HttpDelete]
        [Route("Delete/{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var curso = await _context.Cursos.FindAsync(id);
            if (curso == null)
                return NotFound();

            // Verificar se existem turmas associadas ao curso
            var temTurmas = await _context.Turmas.AnyAsync(t => t.CursoFK == id);
            if (temTurmas)
                return BadRequest(new { message = "Não é possível excluir este curso pois existem turmas associadas a ele" });

            _context.Cursos.Remove(curso);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
