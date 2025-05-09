using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Backend.Data;
using Backend.Models;

namespace Backend.Controllers
{
    public class BlocoHorariosController : Controller
    {
        private readonly ApplicationDbContext _context;

        public BlocoHorariosController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: BlocoHorarios
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.BlocosHorario.Include(b => b.Professor).Include(b => b.Sala).Include(b => b.Turma).Include(b => b.UnidadeCurricular);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: BlocoHorarios/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var blocoHorario = await _context.BlocosHorario
                .Include(b => b.Professor)
                .Include(b => b.Sala)
                .Include(b => b.Turma)
                .Include(b => b.UnidadeCurricular)
                .FirstOrDefaultAsync(m => m.IdBloco == id);
            if (blocoHorario == null)
            {
                return NotFound();
            }

            return View(blocoHorario);
        }

        // GET: BlocoHorarios/Create
        public IActionResult Create()
        {
            ViewData["ProfessorFK"] = new SelectList(_context.Utilizadores, "IdUtilizador", "IdUtilizador");
            ViewData["SalaFK"] = new SelectList(_context.Salas, "IdSala", "IdSala");
            ViewData["TurmaFK"] = new SelectList(_context.Turmas, "IdTurma", "IdTurma");
            ViewData["UnidadeCurricularFK"] = new SelectList(_context.UCs, "IdUC", "IdUC");
            return View();
        }

        // POST: BlocoHorarios/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("IdBloco,HoraInicio,HoraFim,Dia,ProfessorFK,UnidadeCurricularFK,SalaFK,TurmaFK")] BlocoHorario blocoHorario)
        {
            if (ModelState.IsValid)
            {
                _context.Add(blocoHorario);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["ProfessorFK"] = new SelectList(_context.Utilizadores, "IdUtilizador", "IdUtilizador", blocoHorario.ProfessorFK);
            ViewData["SalaFK"] = new SelectList(_context.Salas, "IdSala", "IdSala", blocoHorario.SalaFK);
            ViewData["TurmaFK"] = new SelectList(_context.Turmas, "IdTurma", "IdTurma", blocoHorario.TurmaFK);
            ViewData["UnidadeCurricularFK"] = new SelectList(_context.UCs, "IdUC", "IdUC", blocoHorario.UnidadeCurricularFK);
            return View(blocoHorario);
        }

        // GET: BlocoHorarios/Edit/5
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
            ViewData["ProfessorFK"] = new SelectList(_context.Utilizadores, "IdUtilizador", "IdUtilizador", blocoHorario.ProfessorFK);
            ViewData["SalaFK"] = new SelectList(_context.Salas, "IdSala", "IdSala", blocoHorario.SalaFK);
            ViewData["TurmaFK"] = new SelectList(_context.Turmas, "IdTurma", "IdTurma", blocoHorario.TurmaFK);
            ViewData["UnidadeCurricularFK"] = new SelectList(_context.UCs, "IdUC", "IdUC", blocoHorario.UnidadeCurricularFK);
            return View(blocoHorario);
        }

        // POST: BlocoHorarios/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("IdBloco,HoraInicio,HoraFim,Dia,ProfessorFK,UnidadeCurricularFK,SalaFK,TurmaFK")] BlocoHorario blocoHorario)
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
            ViewData["ProfessorFK"] = new SelectList(_context.Utilizadores, "IdUtilizador", "IdUtilizador", blocoHorario.ProfessorFK);
            ViewData["SalaFK"] = new SelectList(_context.Salas, "IdSala", "IdSala", blocoHorario.SalaFK);
            ViewData["TurmaFK"] = new SelectList(_context.Turmas, "IdTurma", "IdTurma", blocoHorario.TurmaFK);
            ViewData["UnidadeCurricularFK"] = new SelectList(_context.UCs, "IdUC", "IdUC", blocoHorario.UnidadeCurricularFK);
            return View(blocoHorario);
        }

        // GET: BlocoHorarios/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var blocoHorario = await _context.BlocosHorario
                .Include(b => b.Professor)
                .Include(b => b.Sala)
                .Include(b => b.Turma)
                .Include(b => b.UnidadeCurricular)
                .FirstOrDefaultAsync(m => m.IdBloco == id);
            if (blocoHorario == null)
            {
                return NotFound();
            }

            return View(blocoHorario);
        }

        // POST: BlocoHorarios/Delete/5
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
