using FaiscaMcQueen_Transportes.Data;
using FaiscaMcQueen_Transportes.Data.FaiscaMcQueen;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FaiscaMcQueen_Transportes.Controllers
{
    public class AtivosController : Controller
    {
        private readonly FaiscaMcQueenContext _context;

        public AtivosController(FaiscaMcQueenContext context)
        {
            _context = context;
        }

       
        public async Task<IActionResult> Index()
        {
            return View(await _context.Ativos.ToListAsync());
        }

        
        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null)
                return NotFound();

            var ativo = await _context.Ativos
                .FirstOrDefaultAsync(m => m.Id == id);

            if (ativo == null)
                return NotFound();

            return View(ativo);
        }

       
        public IActionResult Create()
        {
            return View();
        }

        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Ativo ativo)
        {
            if (ModelState.IsValid)
            {
                ativo.Id = Guid.NewGuid();
                _context.Add(ativo);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            return View(ativo);
        }

        
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null)
                return NotFound();

            var ativo = await _context.Ativos.FindAsync(id);

            if (ativo == null)
                return NotFound();

            return View(ativo);
        }

        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, Ativo ativo)
        {
            if (id != ativo.Id)
                return NotFound();

            if (ModelState.IsValid)
            {
                _context.Update(ativo);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            return View(ativo);
        }

        
        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null)
                return NotFound();

            var ativo = await _context.Ativos
                .FirstOrDefaultAsync(m => m.Id == id);

            if (ativo == null)
                return NotFound();

            return View(ativo);
        }

       
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var ativo = await _context.Ativos.FindAsync(id);

            if (ativo != null)
            {
                _context.Ativos.Remove(ativo);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }
    }
}
