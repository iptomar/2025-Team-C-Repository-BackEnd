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


            // Carregar entidades relacionadas para criar o DTO
            await _context.Entry(blocoHorario).Reference(b => b.Professor).LoadAsync();
            await _context.Entry(blocoHorario).Reference(b => b.Disciplina).LoadAsync();
            await _context.Entry(blocoHorario).Reference(b => b.Sala).LoadAsync();
            await _context.Entry(blocoHorario).Reference(b => b.Tipologia).LoadAsync();
            await _context.Entry(blocoHorario).Reference(b => b.Turma).LoadAsync();

            // Criar DTO para enviar ao cliente
            var blocoDTO = new BlocoHorarioDTO
            {
                IdBloco = blocoHorario.IdBloco,
                HoraInicio = blocoHorario.HoraInicio,
                HoraFim = blocoHorario.HoraFim,
                DiaSemana = blocoHorario.DiaSemana,
                ProfessorNome = blocoHorario.Professor.Nome,
                DisciplinaNome = blocoHorario.Disciplina.NomeDisciplina,
                SalaNome = blocoHorario.Sala.Nome,
                Tipologia = blocoHorario.Tipologia.Tipologia,
                TurmaNome = blocoHorario.Turma.Nome
            };

            // Notificar os clientes conectados com os dados do bloco
            await _hubContext.Clients.All.SendAsync("BlocoAdicionado", blocoDTO);

            return CreatedAtAction(nameof(GetById), new { id = blocoHorario.IdBloco }, blocoDTO);
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

                // Recarregar o bloco com suas relações
                await _context.Entry(blocoHorario).Reference(b => b.Professor).LoadAsync();
                await _context.Entry(blocoHorario).Reference(b => b.Disciplina).LoadAsync();
                await _context.Entry(blocoHorario).Reference(b => b.Sala).LoadAsync();
                await _context.Entry(blocoHorario).Reference(b => b.Tipologia).LoadAsync();
                await _context.Entry(blocoHorario).Reference(b => b.Turma).LoadAsync();

                // Criar DTO para enviar ao cliente
                var blocoDTO = new BlocoHorarioDTO
                {
                    IdBloco = blocoHorario.IdBloco,
                    HoraInicio = blocoHorario.HoraInicio,
                    HoraFim = blocoHorario.HoraFim,
                    DiaSemana = blocoHorario.DiaSemana,
                    ProfessorNome = blocoHorario.Professor.Nome,
                    DisciplinaNome = blocoHorario.Disciplina.NomeDisciplina,
                    SalaNome = blocoHorario.Sala.Nome,
                    Tipologia = blocoHorario.Tipologia.Tipologia,
                    TurmaNome = blocoHorario.Turma.Nome
                };

                // Notificar os clientes conectados
                await _hubContext.Clients.All.SendAsync("BlocoEditado", blocoDTO);

                return NoContent();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.BlocosHorario.Any(b => b.IdBloco == id))
                    return NotFound();
                else
                    throw;
            }
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

            // Notificar os clientes sobre a exclusão
            await _hubContext.Clients.All.SendAsync("BlocoExcluido", id);

            return NoContent();
        }
    }
}
