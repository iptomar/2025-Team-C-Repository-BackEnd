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
    public class UCsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public UCsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: UCs
        public async Task<IActionResult> Index()
        {
            return View(await _context.UCs.ToListAsync());
        }

        // GET: UCs/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var uC = await _context.UCs
                .FirstOrDefaultAsync(m => m.IdDisciplina == id);
            if (uC == null)
            {
                return NotFound();
            }

            return View(uC);
        }

        // GET: UCs/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: UCs/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("IdDisciplina,NomeDisciplina,TipoDisciplina,GrauAcademico,Tipologia,Semestre")] UC uC)
        {
            if (ModelState.IsValid)
            {
                _context.Add(uC);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(uC);
        }

        // GET: UCs/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var uC = await _context.UCs.FindAsync(id);
            if (uC == null)
            {
                return NotFound();
            }
            return View(uC);
        }

        // POST: UCs/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("IdDisciplina,NomeDisciplina,TipoDisciplina,GrauAcademico,Tipologia,Semestre")] UC uC)
        {
            if (id != uC.IdDisciplina)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(uC);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!UCExists(uC.IdDisciplina))
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
            return View(uC);
        }

        // GET: UCs/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var uC = await _context.UCs
                .FirstOrDefaultAsync(m => m.IdDisciplina == id);
            if (uC == null)
            {
                return NotFound();
            }

            return View(uC);
        }

        // POST: UCs/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var uC = await _context.UCs.FindAsync(id);
            if (uC != null)
            {
                _context.UCs.Remove(uC);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool UCExists(int id)
        {
            return _context.UCs.Any(e => e.IdDisciplina == id);
        }
    }
}
