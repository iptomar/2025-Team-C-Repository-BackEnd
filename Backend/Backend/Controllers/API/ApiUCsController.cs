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
                    IdUC = u.IdUC,
                    NomeUC = u.NomeUC,
                    TipoUC = u.TipoUC,
                    GrauAcademico = u.GrauAcademico,
                    Tipologia = u.Tipologia,
                    Semestre = u.Semestre,
                    Ano = u.Ano,
                    CursoFK = u.CursoFK
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
                .FirstOrDefaultAsync(u => u.IdUC == id);

            if (uc == null)
                return NotFound();

            var ucDTO = new UCDTO
            {
                IdUC = uc.IdUC,
                NomeUC = uc.NomeUC,
                TipoUC = uc.TipoUC,
                GrauAcademico = uc.GrauAcademico,
                Tipologia = uc.Tipologia,
                Semestre = uc.Semestre,
                Ano = uc.Ano,
                CursoFK = uc.CursoFK
            };

            return Ok(ucDTO);
        }

        /// <summary>
        /// Cria uma nova unidade curricular.
        /// </summary>
        [HttpPost]
        [Route("Create")]
        public async Task<ActionResult<UCDTO>> Create([FromBody] UCDTO ucDTO)
        {
            var uc = new UC
            {
                NomeUC = ucDTO.NomeUC,
                TipoUC = ucDTO.TipoUC,
                GrauAcademico = ucDTO.GrauAcademico,
                Tipologia = ucDTO.Tipologia,
                Semestre = ucDTO.Semestre,
                Ano = ucDTO.Ano,
                CursoFK = ucDTO.CursoFK
            };

            _context.UCs.Add(uc);
            await _context.SaveChangesAsync();

            ucDTO.IdUC = uc.IdUC;

            return CreatedAtAction(nameof(GetById), new { id = uc.IdUC }, ucDTO);
        }

        /// <summary>
        /// Atualiza uma unidade curricular existente.
        /// </summary>
        [HttpPut]
        [Route("Update/{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] UCDTO ucDTO)
        {
            if (id != ucDTO.IdUC)
                return BadRequest();

            var uc = await _context.UCs.FindAsync(id);
            if (uc == null)
                return NotFound();

            uc.NomeUC = ucDTO.NomeUC;
            uc.TipoUC = ucDTO.TipoUC;
            uc.GrauAcademico = ucDTO.GrauAcademico;
            uc.Tipologia = ucDTO.Tipologia;
            uc.Semestre = ucDTO.Semestre;
            uc.Ano = ucDTO.Ano;
            uc.CursoFK = ucDTO.CursoFK;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.UCs.Any(u => u.IdUC == id))
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

            // Verificar se existem blocos de horário associados à UC
            var temBlocosHorario = await _context.BlocosHorario
                .AnyAsync(b => b.UnidadeCurricularFK == id);

            if (temBlocosHorario)
                return BadRequest(new { message = "Não é possível excluir esta disciplina pois existem blocos de horário associados a ela" });

            _context.UCs.Remove(uc);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
