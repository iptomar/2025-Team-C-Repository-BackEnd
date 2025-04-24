using Backend.Data;
using Backend.DTO;
using Backend.Hubs;
using Backend.Models;
using Backend.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

namespace Backend.Controllers.API
{
    [Route("api/[controller]")]
    [ApiController]
    public class ApiBlocosHorarioController : ControllerBase
    {
        /// <summary>
        /// Permite a interação com a base de dados.
        /// </summary>
        private readonly ApplicationDbContext _context;

        /// <summary>
        /// Permite fazer a verificação de blocos de horário.
        /// </summary>
        private readonly HorarioValidator _horarioValidator;

        /// <summary>
        /// Permite a comunicação em tempo real com o cliente.
        /// </summary>
        private readonly IHubContext<HorarioHub> _hubContext;

        /// <summary>
        /// Construtor
        /// </summary>
        public ApiBlocosHorarioController(ApplicationDbContext context, HorarioValidator horarioValidator, IHubContext<HorarioHub> hubContext)
        {
            _context = context;
            _horarioValidator = horarioValidator;
            _hubContext = hubContext;
        }

        /// <summary>
        /// Retorna todos os blocos de horário.
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("getAll")]
        public async Task<ActionResult<IEnumerable<BlocoHorarioDTO>>> GetAll()
        {
            var blocos = await _context.BlocosHorario
                .Include(b => b.Disciplina)
                .Include(b => b.Professor)
                .Include(b => b.Sala)
                .Include(b => b.Tipologia)
                .Include(b => b.Turma)
                .Select(b => new BlocoHorarioDTO
                {
                    IdBloco = b.IdBloco,
                    HoraInicio = b.HoraInicio,
                    HoraFim = b.HoraFim,
                    DiaSemana = b.DiaSemana,
                    ProfessorNome = b.Professor.Nome,
                    DisciplinaNome = b.Disciplina.NomeDisciplina,
                    SalaNome = b.Sala.Nome,
                    Tipologia = b.Tipologia.Tipologia,
                    TurmaNome = b.Turma.Nome
                })
                .ToListAsync();

            return Ok(blocos);
        }

        /// <summary>
        /// Retorna um bloco de horário específico pelo ID.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id}")]
        [Route("getById/{id}")]
        public async Task<ActionResult<BlocoHorarioDTO>> GetById(int id)
        {
            var bloco = await _context.BlocosHorario
                .Include(b => b.Disciplina)
                .Include(b => b.Professor)
                .Include(b => b.Sala)
                .Include(b => b.Tipologia)
                .Include(b => b.Turma)
                .FirstOrDefaultAsync(b => b.IdBloco == id);

            if (bloco == null) return NotFound();

            var dto = new BlocoHorarioDTO
            {
                IdBloco = bloco.IdBloco,
                HoraInicio = bloco.HoraInicio,
                HoraFim = bloco.HoraFim,
                DiaSemana = bloco.DiaSemana,
                ProfessorNome = bloco.Professor.Nome,
                DisciplinaNome = bloco.Disciplina.NomeDisciplina,
                SalaNome = bloco.Sala.Nome,
                Tipologia = bloco.Tipologia.Tipologia,
                TurmaNome = bloco.Turma.Nome
            };

            return Ok(dto);
        }

        /// <summary>
        /// Cria um novo bloco de horário.
        /// </summary>
        /// <param name="blocoHorario"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("create")]
        public async Task<IActionResult> Create([FromBody] BlocoHorario blocoHorario)
        {
            // Validar o bloco de horário
            var validacao = await _horarioValidator.ValidarBlocoHorario(blocoHorario);
            if (!validacao.isValid)
                return BadRequest(new { message = validacao.errorMessage });

            _context.BlocosHorario.Add(blocoHorario);
            await _context.SaveChangesAsync();


            // Notificar os clientes conectados
            await _hubContext.Clients.All.SendAsync("ReceberAtualizacao", "Bloco de horário criado.");

            return CreatedAtAction(nameof(GetById), new { id = blocoHorario.IdBloco }, blocoHorario);
        }

        /// <summary>
        /// Atualiza um bloco de horário existente.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="blocoHorario"></param>
        /// <returns></returns>
        [HttpPut("{id}")]
        [Route("update/{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] BlocoHorario blocoHorario)
        {
            if (id != blocoHorario.IdBloco)
                return BadRequest();


            // Validar o bloco de horário
            var validacao = await _horarioValidator.ValidarBlocoHorario(blocoHorario);
            if (!validacao.isValid)
                return BadRequest(new { message = validacao.errorMessage });

            _context.Entry(blocoHorario).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.BlocosHorario.Any(b => b.IdBloco == id))
                    return NotFound();
                else
                    throw;
            }

            return NoContent();
        }

        /// <summary>
        /// Remove um bloco de horário pelo ID.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete("{id}")]
        [Route("delete/{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var bloco = await _context.BlocosHorario.FindAsync(id);
            if (bloco == null)
                return NotFound();

            _context.BlocosHorario.Remove(bloco);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
