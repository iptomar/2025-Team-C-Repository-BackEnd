using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Backend_v02.Data;
using Backend_v02.Models;

namespace Backend_v02.Controllers
{
    public class BlocosHorarioController : Controller
    {
        private readonly ApplicationDbContext _context;

        public BlocosHorarioController(ApplicationDbContext context)
        {
            _context = context;
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
            ViewData["DisciplinaFK"] = new SelectList(_context.UCs, "IdDisciplina", "IdDisciplina");
            ViewData["ProfessorFK"] = new SelectList(_context.Utilizadores, "IdUtilizador", "IdUtilizador");
            ViewData["SalaFK"] = new SelectList(_context.Salas, "IdSala", "IdSala");
            ViewData["TipologiaFK"] = new SelectList(_context.UCs, "IdDisciplina", "IdDisciplina");
            ViewData["TurmaFK"] = new SelectList(_context.Turmas, "IdTurma", "IdTurma");
            return View();
        }

        // POST: BlocosHorario/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("IdBloco,HoraInicio,HoraFim,DiaSemana,ProfessorFK,DisciplinaFK,SalaFK,TurmaFK,TipologiaFK")] BlocoHorario blocoHorario)
        {
            if (ModelState.IsValid)
            {
                _context.Add(blocoHorario);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["DisciplinaFK"] = new SelectList(_context.UCs, "IdDisciplina", "IdDisciplina", blocoHorario.DisciplinaFK);
            ViewData["ProfessorFK"] = new SelectList(_context.Utilizadores, "IdUtilizador", "IdUtilizador", blocoHorario.ProfessorFK);
            ViewData["SalaFK"] = new SelectList(_context.Salas, "IdSala", "IdSala", blocoHorario.SalaFK);
            ViewData["TipologiaFK"] = new SelectList(_context.UCs, "IdDisciplina", "IdDisciplina", blocoHorario.TipologiaFK);
            ViewData["TurmaFK"] = new SelectList(_context.Turmas, "IdTurma", "IdTurma", blocoHorario.TurmaFK);
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
            ViewData["DisciplinaFK"] = new SelectList(_context.UCs, "IdDisciplina", "IdDisciplina", blocoHorario.DisciplinaFK);
            ViewData["ProfessorFK"] = new SelectList(_context.Utilizadores, "IdUtilizador", "IdUtilizador", blocoHorario.ProfessorFK);
            ViewData["SalaFK"] = new SelectList(_context.Salas, "IdSala", "IdSala", blocoHorario.SalaFK);
            ViewData["TipologiaFK"] = new SelectList(_context.UCs, "IdDisciplina", "IdDisciplina", blocoHorario.TipologiaFK);
            ViewData["TurmaFK"] = new SelectList(_context.Turmas, "IdTurma", "IdTurma", blocoHorario.TurmaFK);
            return View(blocoHorario);
        }

        // POST: BlocosHorario/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
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
                try
                {
                    _context.Update(blocoHorario);
                    await _context.SaveChangesAsync();
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
                return RedirectToAction(nameof(Index));
            }
            ViewData["DisciplinaFK"] = new SelectList(_context.UCs, "IdDisciplina", "IdDisciplina", blocoHorario.DisciplinaFK);
            ViewData["ProfessorFK"] = new SelectList(_context.Utilizadores, "IdUtilizador", "IdUtilizador", blocoHorario.ProfessorFK);
            ViewData["SalaFK"] = new SelectList(_context.Salas, "IdSala", "IdSala", blocoHorario.SalaFK);
            ViewData["TipologiaFK"] = new SelectList(_context.UCs, "IdDisciplina", "IdDisciplina", blocoHorario.TipologiaFK);
            ViewData["TurmaFK"] = new SelectList(_context.Turmas, "IdTurma", "IdTurma", blocoHorario.TurmaFK);
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
