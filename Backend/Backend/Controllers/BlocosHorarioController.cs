using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Backend.Data;
using Backend.Models;
using Backend.Services;

namespace Backend.Controllers
{
    public class BlocosHorarioController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly HorarioValidator _horarioValidator;

        public BlocosHorarioController(ApplicationDbContext context, HorarioValidator horarioValidator)
        {
            _context = context;
            _horarioValidator = horarioValidator;
        }

        // GET: BlocosHorario
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.BlocosHorario.Include(b => b.Disciplina).Include(b => b.Professor).Include(b => b.Sala).Include(b => b.Tipologia).Include(b => b.Turma);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: BlocosHorario/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var blocoHorario = await _context.BlocosHorario
                .Include(b => b.Disciplina)
                .Include(b => b.Professor)
                .Include(b => b.Sala)
                .Include(b => b.Tipologia)
                .Include(b => b.Turma)
                .FirstOrDefaultAsync(m => m.IdBloco == id);
            if (blocoHorario == null)
            {
                return NotFound();
            }

            return View(blocoHorario);
        }

        // GET: BlocosHorario/Create
        public IActionResult Create()
        {
            // Criar lista de opções de meia em meia hora (08:00 às 23:30)
            var timeOptions = new List<SelectListItem>();
            for (int hour = 8; hour < 24; hour++)
            {
                timeOptions.Add(new SelectListItem { Value = new TimeSpan(hour, 0, 0).ToString(), Text = $"{hour:00}:00" });
                timeOptions.Add(new SelectListItem { Value = new TimeSpan(hour, 30, 0).ToString(), Text = $"{hour:00}:30" });
            }

            ViewBag.TimeOptions = timeOptions;

            // Criar lista de dias da semana
            ViewBag.DiaSemanaOptions = new SelectList(new[]
            {
                new { Value = DayOfWeek.Monday, Text = "Segunda-feira" },
                new { Value = DayOfWeek.Tuesday, Text = "Terça-feira" },
                new { Value = DayOfWeek.Wednesday, Text = "Quarta-feira" },
                new { Value = DayOfWeek.Thursday, Text = "Quinta-feira" },
                new { Value = DayOfWeek.Friday, Text = "Sexta-feira" },
                new { Value = DayOfWeek.Saturday, Text = "Sábado" },
                new { Value = DayOfWeek.Sunday, Text = "Domingo" }
            }, "Value", "Text");

            ViewData["DisciplinaFK"] = new SelectList(_context.UCs, "IdDisciplina", "NomeDisciplina");
            ViewData["ProfessorFK"] = new SelectList(_context.Utilizadores, "IdUtilizador", "Nome");
            ViewData["SalaFK"] = new SelectList(_context.Salas, "IdSala", "Nome");
            ViewData["TipologiaFK"] = new SelectList(_context.UCs, "IdDisciplina", "Tipologia");
            ViewData["TurmaFK"] = new SelectList(_context.Turmas, "IdTurma", "Nome");
            return View();
        }

        // POST: BlocosHorario/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("IdBloco,HoraInicio,HoraFim,DiaSemana,ProfessorFK,DisciplinaFK,SalaFK,TurmaFK,TipologiaFK")] BlocoHorario blocoHorario)
        {
            if (ModelState.IsValid)
            {
                // Validar o bloco de horário
                var validacao = await _horarioValidator.ValidarBlocoHorario(blocoHorario);

                if (!validacao.isValid)
                {
                    ModelState.AddModelError(string.Empty, validacao.errorMessage);
                }
                else
                {
                    _context.Add(blocoHorario);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
            }

            // Se chegou aqui, algo está inválido, recarregar os dados para a view
            ViewData["DisciplinaFK"] = new SelectList(_context.UCs, "IdDisciplina", "NomeDisciplina", blocoHorario.DisciplinaFK);
            ViewData["ProfessorFK"] = new SelectList(_context.Utilizadores, "IdUtilizador", "Nome", blocoHorario.ProfessorFK);
            ViewData["SalaFK"] = new SelectList(_context.Salas, "IdSala", "Nome", blocoHorario.SalaFK);
            ViewData["TipologiaFK"] = new SelectList(_context.UCs, "IdDisciplina", "Tipologia", blocoHorario.TipologiaFK);
            ViewData["TurmaFK"] = new SelectList(_context.Turmas, "IdTurma", "Nome", blocoHorario.TurmaFK);
            return View(blocoHorario);
        }

        // GET: BlocosHorario/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var blocoHorario = await _context.BlocosHorario.FindAsync(id);
            if (blocoHorario == null)
            {
                return NotFound();
            }

            // Criar lista de opções de meia em meia hora (08:00 às 23:30)
            var timeOptions = new List<SelectListItem>();
            for (int hour = 8; hour < 24; hour++)
            {
                timeOptions.Add(new SelectListItem
                {
                    Value = new TimeSpan(hour, 0, 0).ToString(),
                    Text = $"{hour:00}:00"
                });
                timeOptions.Add(new SelectListItem
                {
                    Value = new TimeSpan(hour, 30, 0).ToString(),
                    Text = $"{hour:00}:30"
                });
            }

            ViewBag.TimeOptions = timeOptions;

            // Criar lista de dias da semana
            ViewBag.DiaSemanaOptions = new SelectList(new[]
            {
                new { Value = DayOfWeek.Monday, Text = "Segunda-feira" },
                new { Value = DayOfWeek.Tuesday, Text = "Terça-feira" },
                new { Value = DayOfWeek.Wednesday, Text = "Quarta-feira" },
                new { Value = DayOfWeek.Thursday, Text = "Quinta-feira" },
                new { Value = DayOfWeek.Friday, Text = "Sexta-feira" },
                new { Value = DayOfWeek.Saturday, Text = "Sábado" },
                new { Value = DayOfWeek.Sunday, Text = "Domingo" }
            }, "Value", "Text", blocoHorario.DiaSemana);

            ViewData["DisciplinaFK"] = new SelectList(_context.UCs, "IdDisciplina", "NomeDisciplina", blocoHorario.DisciplinaFK);
            ViewData["ProfessorFK"] = new SelectList(_context.Utilizadores, "IdUtilizador", "Nome", blocoHorario.ProfessorFK);
            ViewData["SalaFK"] = new SelectList(_context.Salas, "IdSala", "Nome", blocoHorario.SalaFK);
            ViewData["TipologiaFK"] = new SelectList(_context.UCs, "IdDisciplina", "Tipologia", blocoHorario.TipologiaFK);
            ViewData["TurmaFK"] = new SelectList(_context.Turmas, "IdTurma", "Nome", blocoHorario.TurmaFK);
            return View(blocoHorario);
        }

        // POST: BlocosHorario/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("IdBloco,HoraInicio,HoraFim,DiaSemana,ProfessorFK,DisciplinaFK,SalaFK,TurmaFK,TipologiaFK")] BlocoHorario blocoHorario)
        {
            if (id != blocoHorario.IdBloco)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                // Validar o bloco de horário, excluindo o próprio bloco da validação
                var validacao = await _horarioValidator.ValidarBlocoHorario(blocoHorario, id);

                if (!validacao.isValid)
                {
                    ModelState.AddModelError(string.Empty, validacao.errorMessage);
                }
                else
                {
                    try
                    {
                        _context.Update(blocoHorario);
                        await _context.SaveChangesAsync();
                        return RedirectToAction(nameof(Index));
                    }
                    catch (DbUpdateConcurrencyException)
                    {
                        if (!BlocoHorarioExists(blocoHorario.IdBloco))
                        {
                            return NotFound();
                        }
                        else
                        {
                            throw;
                        }
                    }
                }
            }

            // Se chegou aqui, algo está inválido, recarregar os dados para a view
            ViewData["DisciplinaFK"] = new SelectList(_context.UCs, "IdDisciplina", "NomeDisciplina", blocoHorario.DisciplinaFK);
            ViewData["ProfessorFK"] = new SelectList(_context.Utilizadores, "IdUtilizador", "Nome", blocoHorario.ProfessorFK);
            ViewData["SalaFK"] = new SelectList(_context.Salas, "IdSala", "Nome", blocoHorario.SalaFK);
            ViewData["TipologiaFK"] = new SelectList(_context.UCs, "IdDisciplina", "Tipologia", blocoHorario.TipologiaFK);
            ViewData["TurmaFK"] = new SelectList(_context.Turmas, "IdTurma", "Nome", blocoHorario.TurmaFK);
            return View(blocoHorario);
        }

        // GET: BlocosHorario/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var blocoHorario = await _context.BlocosHorario
                .Include(b => b.Disciplina)
                .Include(b => b.Professor)
                .Include(b => b.Sala)
                .Include(b => b.Tipologia)
                .Include(b => b.Turma)
                .FirstOrDefaultAsync(m => m.IdBloco == id);
            if (blocoHorario == null)
            {
                return NotFound();
            }

            return View(blocoHorario);
        }

        // POST: BlocosHorario/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var blocoHorario = await _context.BlocosHorario.FindAsync(id);
            if (blocoHorario != null)
            {
                _context.BlocosHorario.Remove(blocoHorario);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool BlocoHorarioExists(int id)
        {
            return _context.BlocosHorario.Any(e => e.IdBloco == id);
        }
    }
}
