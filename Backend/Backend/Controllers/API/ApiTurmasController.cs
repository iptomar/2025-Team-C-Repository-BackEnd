using Backend.Data;
using Backend.DTO;
using Backend.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Backend.Controllers.API
{
    [Route("api/[controller]")]
    [ApiController]
    public class ApiTurmasController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public ApiTurmasController(ApplicationDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Retorna todas as turmas.
        /// </summary>
        [HttpGet]
        [Route("GetAll")]
        public async Task<ActionResult<IEnumerable<TurmaDTO>>> GetTurmas()
        {
            var turmas = await _context.Turmas
                .Include(t => t.Curso)
                .Select(t => new TurmaDTO
                {
                    IdTurma = t.IdTurma,
                    Nome = t.Nome,
                    CursoFK = t.CursoFK,
                    CursoNome = t.Curso.Nome
                })
                .ToListAsync();

            return Ok(turmas);
        }

        /// <summary>
        /// Retorna todas as turmas de um curso específico.
        /// </summary>
        [HttpGet]
        [Route("GetByCurso/{cursoId}")]
        public async Task<ActionResult<IEnumerable<TurmaDTO>>> GetTurmasByCurso(int cursoId)
        {
            var turmas = await _context.Turmas
                .Where(t => t.CursoFK == cursoId)
                .Include(t => t.Curso)
                .Select(t => new TurmaDTO
                {
                    IdTurma = t.IdTurma,
                    Nome = t.Nome,
                    CursoFK = t.CursoFK,
                    CursoNome = t.Curso.Nome
                })
                .ToListAsync();

            return Ok(turmas);
        }

        /// <summary>
        /// Retorna uma turma pelo ID.
        /// </summary>
        [HttpGet]
        [Route("GetById/{id}")]
        public async Task<ActionResult<TurmaDTO>> GetById(int id)
        {
            var turma = await _context.Turmas
                .Include(t => t.Curso)
                .Include(t => t.BlocosHorario)
                .FirstOrDefaultAsync(t => t.IdTurma == id);

            if (turma == null)
                return NotFound();

            var turmaDTO = new TurmaDTO
            {
                IdTurma = turma.IdTurma,
                Nome = turma.Nome,
                CursoFK = turma.CursoFK,
                CursoNome = turma.Curso?.Nome
            };

            return Ok(turmaDTO);
        }

        /// <summary>
        /// Cria uma nova turma.
        /// </summary>
        [HttpPost]
        [Route("Create")]
        public async Task<ActionResult<TurmaDTO>> Create([FromBody] TurmaDTO turmaDTO)
        {
            // Verificar se o curso existe
            var curso = await _context.Cursos.FindAsync(turmaDTO.CursoFK);
            if (curso == null)
                return BadRequest(new { message = "Curso não encontrado" });

            // Criar uma nova turma com os dados do DTO
            var turma = new Turma
            {
                Nome = turmaDTO.Nome,
                CursoFK = turmaDTO.CursoFK
            };

            _context.Turmas.Add(turma);
            await _context.SaveChangesAsync();

            // Atualizar o DTO com o ID gerado
            turmaDTO.IdTurma = turma.IdTurma;
            turmaDTO.CursoNome = curso.Nome;

            return CreatedAtAction(nameof(GetById), new { id = turma.IdTurma }, turmaDTO);
        }

        /// <summary>
        /// Atualiza uma turma existente.
        /// </summary>
        [HttpPut]
        [Route("Update/{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] TurmaDTO turmaDTO)
        {
            if (id != turmaDTO.IdTurma)
                return BadRequest();

            // Verificar se o curso existe
            var curso = await _context.Cursos.FindAsync(turmaDTO.CursoFK);
            if (curso == null)
                return BadRequest(new { message = "Curso não encontrado" });

            // Obter a turma existente
            var turma = await _context.Turmas.FindAsync(id);
            if (turma == null)
                return NotFound();

            // Atualizar os dados da turma
            turma.Nome = turmaDTO.Nome;
            turma.CursoFK = turmaDTO.CursoFK;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.Turmas.Any(t => t.IdTurma == id))
                    return NotFound();
                else
                    throw;
            }

            return NoContent();
        }

        /// <summary>
        /// Remove uma turma pelo ID.
        /// </summary>
        [HttpDelete]
        [Route("Delete/{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var turma = await _context.Turmas.FindAsync(id);
            if (turma == null)
                return NotFound();

            // Verificar se existem blocos de horário associados à turma
            var temBlocosHorario = await _context.BlocosHorario
                .AnyAsync(b => b.TurmaFK == id);

            if (temBlocosHorario)
                return BadRequest(new { message = "Não é possível excluir esta turma pois existem blocos de horário associados a ela" });

            _context.Turmas.Remove(turma);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
