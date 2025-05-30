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
                .Include(b => b.UnidadeCurricular)
                .Include(b => b.Professor)
                .Include(b => b.Sala)
                .Include(b => b.Turma)
                .Select(b => new BlocoHorarioDTO
                {
                    IdBloco = b.IdBloco,
                    HoraInicio = b.HoraInicio,
                    HoraFim = b.HoraFim,
                    Dia = b.Dia,
                    ProfessorFK = b.ProfessorFK,
                    ProfessorNome = b.Professor.Nome,
                    UnidadeCurricularFK = b.UnidadeCurricularFK,
                    UnidadeCurricularNome = b.UnidadeCurricular.NomeUC,
                    SalaFK = b.SalaFK,
                    SalaNome = b.Sala.Nome,
                    TurmaFK = b.TurmaFK,
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
        [HttpGet]
        [Route("getById/{id}")]
        public async Task<ActionResult<BlocoHorarioDTO>> GetById(int id)
        {
            var bloco = await _context.BlocosHorario
                .Include(b => b.UnidadeCurricular)
                .Include(b => b.Professor)
                .Include(b => b.Sala)
                .Include(b => b.Turma)
                .FirstOrDefaultAsync(b => b.IdBloco == id);

            if (bloco == null) return NotFound();

            var dto = new BlocoHorarioDTO
            {
                IdBloco = bloco.IdBloco,
                HoraInicio = bloco.HoraInicio,
                HoraFim = bloco.HoraFim,
                Dia = bloco.Dia,
                ProfessorFK = bloco.ProfessorFK,
                ProfessorNome = bloco.Professor.Nome,
                UnidadeCurricularFK = bloco.UnidadeCurricularFK,
                UnidadeCurricularNome = bloco.UnidadeCurricular.NomeUC,
                SalaFK = bloco.SalaFK,
                SalaNome = bloco.Sala.Nome,
                TurmaFK = bloco.TurmaFK,
                TurmaNome = bloco.Turma.Nome
            };

            return Ok(dto);
        }

        /// <summary>
        /// Cria um novo bloco de horário.
        /// </summary>
        /// <param name="blocoDTO">DTO com os dados do bloco horário</param>
        /// <returns></returns>
        [HttpPost]
        [Route("create")]
        public async Task<IActionResult> Create([FromBody] BlocoHorarioDTO blocoDTO)
        {
            // Converter DTO para modelo
            var blocoHorario = new BlocoHorario
            {
                HoraInicio = blocoDTO.HoraInicio,
                HoraFim = blocoDTO.HoraFim,
                Dia = blocoDTO.Dia,
                ProfessorFK = blocoDTO.ProfessorFK,
                UnidadeCurricularFK = blocoDTO.UnidadeCurricularFK,
                SalaFK = blocoDTO.SalaFK,
                TurmaFK = blocoDTO.TurmaFK
            };

            // Validar o bloco de horário
            var validacao = await _horarioValidator.ValidarBlocoHorario(blocoHorario);
            if (!validacao.isValid)
                return BadRequest(new { message = validacao.errorMessage });

            _context.BlocosHorario.Add(blocoHorario);
            await _context.SaveChangesAsync();

            // Carregar entidades relacionadas para criar o DTO de resposta
            await _context.Entry(blocoHorario).Reference(b => b.Professor).LoadAsync();
            await _context.Entry(blocoHorario).Reference(b => b.UnidadeCurricular).LoadAsync();
            await _context.Entry(blocoHorario).Reference(b => b.Sala).LoadAsync();
            await _context.Entry(blocoHorario).Reference(b => b.Turma).LoadAsync();

            // Criar DTO para enviar ao cliente
            var novoBlocoDTO = new BlocoHorarioDTO
            {
                IdBloco = blocoHorario.IdBloco,
                HoraInicio = blocoHorario.HoraInicio,
                HoraFim = blocoHorario.HoraFim,
                Dia = blocoHorario.Dia,
                ProfessorFK = blocoHorario.ProfessorFK,
                ProfessorNome = blocoHorario.Professor.Nome,
                UnidadeCurricularFK = blocoHorario.UnidadeCurricularFK,
                UnidadeCurricularNome = blocoHorario.UnidadeCurricular.NomeUC,
                SalaFK = blocoHorario.SalaFK,
                SalaNome = blocoHorario.Sala.Nome,
                TurmaFK = blocoHorario.TurmaFK,
                TurmaNome = blocoHorario.Turma.Nome
            };

            // Notificar os clientes conectados com os dados do bloco
            await _hubContext.Clients.All.SendAsync("BlocoAdicionado", novoBlocoDTO);

            return CreatedAtAction(nameof(GetById), new { id = blocoHorario.IdBloco }, novoBlocoDTO);
        }

        /// <summary>
        /// Atualiza um bloco de horário existente.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="blocoDTO"></param>
        /// <returns></returns>
        [HttpPut]
        [Route("update/{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] BlocoHorarioDTO blocoDTO)
        {
            if (id != blocoDTO.IdBloco)
                return BadRequest();

            // Verificar se o bloco existe
            var blocoExistente = await _context.BlocosHorario.FindAsync(id);
            if (blocoExistente == null)
                return NotFound();

            // Atualizar as propriedades
            blocoExistente.HoraInicio = blocoDTO.HoraInicio;
            blocoExistente.HoraFim = blocoDTO.HoraFim;
            blocoExistente.Dia = blocoDTO.Dia;
            blocoExistente.ProfessorFK = blocoDTO.ProfessorFK;
            blocoExistente.UnidadeCurricularFK = blocoDTO.UnidadeCurricularFK;
            blocoExistente.SalaFK = blocoDTO.SalaFK;
            blocoExistente.TurmaFK = blocoDTO.TurmaFK;

            // Validar o bloco de horário
            var validacao = await _horarioValidator.ValidarBlocoHorario(blocoExistente, id);
            if (!validacao.isValid)
                return BadRequest(new { message = validacao.errorMessage });

            try
            {
                await _context.SaveChangesAsync();

                // Recarregar o bloco com suas relações
                await _context.Entry(blocoExistente).Reference(b => b.Professor).LoadAsync();
                await _context.Entry(blocoExistente).Reference(b => b.UnidadeCurricular).LoadAsync();
                await _context.Entry(blocoExistente).Reference(b => b.Sala).LoadAsync();
                await _context.Entry(blocoExistente).Reference(b => b.Turma).LoadAsync();

                // Criar DTO para enviar ao cliente
                var blocoAtualizadoDTO = new BlocoHorarioDTO
                {
                    IdBloco = blocoExistente.IdBloco,
                    HoraInicio = blocoExistente.HoraInicio,
                    HoraFim = blocoExistente.HoraFim,
                    Dia = blocoExistente.Dia,
                    ProfessorFK = blocoExistente.ProfessorFK,
                    ProfessorNome = blocoExistente.Professor.Nome,
                    UnidadeCurricularFK = blocoExistente.UnidadeCurricularFK,
                    UnidadeCurricularNome = blocoExistente.UnidadeCurricular.NomeUC,
                    SalaFK = blocoExistente.SalaFK,
                    SalaNome = blocoExistente.Sala.Nome,
                    TurmaFK = blocoExistente.TurmaFK,
                    TurmaNome = blocoExistente.Turma.Nome
                };

                // Notificar os clientes conectados
                await _hubContext.Clients.All.SendAsync("BlocoEditado", blocoAtualizadoDTO);

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
        [HttpDelete]
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
