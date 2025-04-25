using Backend.Data;
using Backend.DTO;
using Backend.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Backend.Controllers.API
{
    [Route("api/[controller]")]
    [ApiController]
    public class ApiUCsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public ApiUCsController(ApplicationDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Retorna todas as unidades curriculares (disciplinas).
        /// </summary>
        [HttpGet]
        [Route("GetAll")]
        public async Task<ActionResult<IEnumerable<UCDTO>>> GetAllUCs()
        {
            var ucs = await _context.UCs
                .Select(u => new UCDTO
                {
                    IdDisciplina = u.IdDisciplina,
                    NomeDisciplina = u.NomeDisciplina,
                    TipoDisciplina = u.TipoDisciplina,
                    GrauAcademico = u.GrauAcademico,
                    Tipologia = u.Tipologia,
                    Semestre = u.Semestre
                })
                .ToListAsync();

            return Ok(ucs);
        }

        /// <summary>
        /// Retorna uma unidade curricular pelo ID.
        /// </summary>
        [HttpGet]
        [Route("GetById/{id}")]
        public async Task<ActionResult<UCDTO>> GetById(int id)
        {
            var uc = await _context.UCs
                .FirstOrDefaultAsync(u => u.IdDisciplina == id);

            if (uc == null)
                return NotFound();

            var ucDTO = new UCDTO
            {
                IdDisciplina = uc.IdDisciplina,
                NomeDisciplina = uc.NomeDisciplina,
                TipoDisciplina = uc.TipoDisciplina,
                GrauAcademico = uc.GrauAcademico,
                Tipologia = uc.Tipologia,
                Semestre = uc.Semestre
            };

            return Ok(ucDTO);
        }

        /// <summary>
        /// Retorna unidades curriculares por semestre.
        /// </summary>
        [HttpGet]
        [Route("GetBySemestre/{semestre}")]
        public async Task<ActionResult<IEnumerable<UCDTO>>> GetBySemestre(string semestre)
        {
            var ucs = await _context.UCs
                .Where(u => u.Semestre == semestre)
                .Select(u => new UCDTO
                {
                    IdDisciplina = u.IdDisciplina,
                    NomeDisciplina = u.NomeDisciplina,
                    TipoDisciplina = u.TipoDisciplina,
                    GrauAcademico = u.GrauAcademico,
                    Tipologia = u.Tipologia,
                    Semestre = u.Semestre
                })
                .ToListAsync();

            return Ok(ucs);
        }

        /// <summary>
        /// Retorna unidades curriculares por tipo de disciplina.
        /// </summary>
        [HttpGet]
        [Route("GetByTipo/{tipo}")]
        public async Task<ActionResult<IEnumerable<UCDTO>>> GetByTipo(string tipo)
        {
            var ucs = await _context.UCs
                .Where(u => u.TipoDisciplina == tipo)
                .Select(u => new UCDTO
                {
                    IdDisciplina = u.IdDisciplina,
                    NomeDisciplina = u.NomeDisciplina,
                    TipoDisciplina = u.TipoDisciplina,
                    GrauAcademico = u.GrauAcademico,
                    Tipologia = u.Tipologia,
                    Semestre = u.Semestre
                })
                .ToListAsync();

            return Ok(ucs);
        }

        /// <summary>
        /// Retorna unidades curriculares por grau acadêmico.
        /// </summary>
        [HttpGet]
        [Route("GetByGrau/{grau}")]
        public async Task<ActionResult<IEnumerable<UCDTO>>> GetByGrau(string grau)
        {
            var ucs = await _context.UCs
                .Where(u => u.GrauAcademico == grau)
                .Select(u => new UCDTO
                {
                    IdDisciplina = u.IdDisciplina,
                    NomeDisciplina = u.NomeDisciplina,
                    TipoDisciplina = u.TipoDisciplina,
                    GrauAcademico = u.GrauAcademico,
                    Tipologia = u.Tipologia,
                    Semestre = u.Semestre
                })
                .ToListAsync();

            return Ok(ucs);
        }

        /// <summary>
        /// Retorna unidades curriculares lecionadas por um determinado docente.
        /// </summary>
        [HttpGet]
        [Route("GetByDocente/{docenteId}")]
        public async Task<ActionResult<IEnumerable<UCDTO>>> GetByDocente(int docenteId)
        {
            var docente = await _context.Utilizadores
                .Include(u => u.DisciplinasLecionadas)
                .FirstOrDefaultAsync(u => u.IdUtilizador == docenteId && u.Funcao == "Docente");

            if (docente == null)
                return NotFound("Docente não encontrado");

            var ucDTOs = docente.DisciplinasLecionadas.Select(d => new UCDTO
            {
                IdDisciplina = d.IdDisciplina,
                NomeDisciplina = d.NomeDisciplina,
                TipoDisciplina = d.TipoDisciplina,
                GrauAcademico = d.GrauAcademico,
                Tipologia = d.Tipologia,
                Semestre = d.Semestre
            }).ToList();

            return Ok(ucDTOs);
        }

        /// <summary>
        /// Cria uma nova unidade curricular.
        /// </summary>
        [HttpPost]
        [Route("Create")]
        public async Task<ActionResult<UCDTO>> Create([FromBody] UCDTO ucDTO)
        {
            UC uc = new UC
            {
                NomeDisciplina = ucDTO.NomeDisciplina,
                TipoDisciplina = ucDTO.TipoDisciplina,
                GrauAcademico = ucDTO.GrauAcademico,
                Tipologia = ucDTO.Tipologia,
                Semestre = ucDTO.Semestre
            };

            _context.UCs.Add(uc);
            await _context.SaveChangesAsync();

            ucDTO.IdDisciplina = uc.IdDisciplina;

            return CreatedAtAction(nameof(GetById), new { id = uc.IdDisciplina }, ucDTO);
        }

        /// <summary>
        /// Atualiza uma unidade curricular existente.
        /// </summary>
        [HttpPut]
        [Route("Update/{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] UCDTO ucDTO)
        {
            if (id != ucDTO.IdDisciplina)
                return BadRequest();

            var uc = await _context.UCs.FindAsync(id);
            if (uc == null)
                return NotFound();

            uc.NomeDisciplina = ucDTO.NomeDisciplina;
            uc.TipoDisciplina = ucDTO.TipoDisciplina;
            uc.GrauAcademico = ucDTO.GrauAcademico;
            uc.Tipologia = ucDTO.Tipologia;
            uc.Semestre = ucDTO.Semestre;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.UCs.Any(u => u.IdDisciplina == id))
                    return NotFound();
                else
                    throw;
            }

            return NoContent();
        }

        /// <summary>
        /// Remove uma unidade curricular pelo ID.
        /// </summary>
        [HttpDelete]
        [Route("Delete/{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var uc = await _context.UCs.FindAsync(id);
            if (uc == null)
                return NotFound();

            // Verificar se existem turmas associadas à UC
            var temTurmas = await _context.Turmas
                .AnyAsync(t => t.DisciplinaFK == id);

            if (temTurmas)
                return BadRequest(new { message = "Não é possível excluir esta disciplina pois existem turmas associadas a ela" });

            // Verificar se existem blocos de horário associados à UC
            var temBlocosHorario = await _context.BlocosHorario
                .AnyAsync(b => b.DisciplinaFK == id);

            if (temBlocosHorario)
                return BadRequest(new { message = "Não é possível excluir esta disciplina pois existem blocos de horário associados a ela" });

            _context.UCs.Remove(uc);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        /// <summary>
        /// Adiciona um docente a uma unidade curricular.
        /// </summary>
        [HttpPost]
        [Route("AddDocente/{ucId}/{docenteId}")]
        public async Task<IActionResult> AddDocente(int ucId, int docenteId)
        {
            var uc = await _context.UCs
                .Include(u => u.Docentes)
                .FirstOrDefaultAsync(u => u.IdDisciplina == ucId);

            if (uc == null)
                return NotFound("Disciplina não encontrada");

            var docente = await _context.Utilizadores
                .FirstOrDefaultAsync(u => u.IdUtilizador == docenteId && u.Funcao == "Docente");

            if (docente == null)
                return NotFound("Docente não encontrado");

            if (uc.Docentes == null)
                uc.Docentes = new List<Utilizador>();

            if (!uc.Docentes.Any(d => d.IdUtilizador == docenteId))
                uc.Docentes.Add(docente);

            await _context.SaveChangesAsync();
            return Ok();
        }

        /// <summary>
        /// Remove um docente de uma unidade curricular.
        /// </summary>
        [HttpDelete]
        [Route("RemoveDocente/{ucId}/{docenteId}")]
        public async Task<IActionResult> RemoveDocente(int ucId, int docenteId)
        {
            var uc = await _context.UCs
                .Include(u => u.Docentes)
                .FirstOrDefaultAsync(u => u.IdDisciplina == ucId);

            if (uc == null)
                return NotFound("Disciplina não encontrada");

            var docente = uc.Docentes?.FirstOrDefault(d => d.IdUtilizador == docenteId);
            if (docente == null)
                return NotFound("Docente não encontrado na disciplina");

            // Verificar se existem blocos de horário associados a este docente nesta UC
            var temBlocosHorario = await _context.BlocosHorario
                .AnyAsync(b => b.DisciplinaFK == ucId && b.ProfessorFK == docenteId);

            if (temBlocosHorario)
                return BadRequest(new { message = "Não é possível remover este docente da disciplina pois existem blocos de horário associados" });

            uc.Docentes.Remove(docente);
            await _context.SaveChangesAsync();
            return Ok();
        }
    }
}
